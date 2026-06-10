import { HttpClient, HttpEvent, HttpEventType, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { concat, from, Observable, of, switchMap } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { ServicePackageImageDto } from './service-packages-api.models';
import { ServicePackagesApiService } from './service-packages-api.service';

export interface ImageUploadProgress {
  progress: number; // 0–100
  done: boolean;
  result?: ServicePackageImageDto;
}

interface CloudinaryUploadResponse {
  public_id: string;
  secure_url: string;
}

@Injectable({ providedIn: 'root' })
export class ServicePackageImageUploadService {
  private http = inject(HttpClient);
  private api = inject(ServicePackagesApiService);

  uploadImage(packageId: number, file: File): Observable<ImageUploadProgress> {
    const hashPromise = file.arrayBuffer().then((buf) =>
      crypto.subtle.digest('SHA-256', buf).then((hashBuf) =>
        [...new Uint8Array(hashBuf)].map((b) => b.toString(16).padStart(2, '0')).join(''),
      ),
    );

    return from(hashPromise).pipe(
      switchMap((fileHash) =>
        this.api.getUploadParams(packageId).pipe(
          switchMap((params) => {
            const formData = new FormData();
            formData.append('file', file);
            formData.append('api_key', params.apiKey);
            formData.append('timestamp', params.timestamp.toString());
            formData.append('signature', params.signature);
            formData.append('folder', params.folder);

            const req = new HttpRequest(
              'POST',
              `https://api.cloudinary.com/v1_1/${params.cloudName}/image/upload`,
              formData,
              { reportProgress: true },
            );

            return this.http.request<CloudinaryUploadResponse>(req).pipe(
              filter(
                (e): e is HttpEvent<CloudinaryUploadResponse> =>
                  e.type === HttpEventType.UploadProgress || e.type === HttpEventType.Response,
              ),
              switchMap((e) => {
                if (e.type === HttpEventType.UploadProgress) {
                  const total = e.total ?? e.loaded;
                  return of<ImageUploadProgress>({
                    progress: Math.round((90 * e.loaded) / total),
                    done: false,
                  });
                }
                const body = (e as any).body as CloudinaryUploadResponse;
                return concat(
                  of<ImageUploadProgress>({ progress: 90, done: false }),
                  this.api
                    .confirmImage(packageId, {
                      publicId: body.public_id,
                      secureUrl: body.secure_url,
                      fileHash,
                    })
                    .pipe(map((dto) => ({ progress: 100, done: true, result: dto } as ImageUploadProgress))),
                );
              }),
            );
          }),
        ),
      ),
    );
  }
}
