import { HttpClient, HttpEvent, HttpEventType, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { concat, from, Observable, of, switchMap } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateServicePackageCommand,
  GetAvailableAddonsResponse,
  GetServicePackageByIdQueryDto,
  ListServicePackagesRequest,
  ListServicePackagesResponse,
  ServicePackageImageDto,
  UpdateServicePackageCommand,
} from './service-packages-api.models';

export interface ImageUploadProgress {
  progress: number;   // 0–100
  done: boolean;
  result?: ServicePackageImageDto;
}

interface CloudinaryUploadParams {
  cloudName: string;
  apiKey: string;
  timestamp: number;
  signature: string;
  folder: string;
}

interface CloudinaryUploadResponse {
  public_id: string;
  secure_url: string;
}

@Injectable({ providedIn: 'root' })
export class ServicePackagesApiService {
  private readonly baseUrl = `${environment.apiUrl}/ServicePackages`;
  private http = inject(HttpClient);

  list(request?: ListServicePackagesRequest): Observable<ListServicePackagesResponse> {
    const params = buildHttpParams((request ?? new ListServicePackagesRequest()) as any);
    return this.http.get<ListServicePackagesResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetServicePackageByIdQueryDto> {
    return this.http.get<GetServicePackageByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(command: CreateServicePackageCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateServicePackageCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getAvailableAddons(servicePackageId: number): Observable<GetAvailableAddonsResponse> {
    return this.http.get<GetAvailableAddonsResponse>(`${this.baseUrl}/available-addons`, {
      params: {
        servicePackageId: servicePackageId.toString(),
        'paging.pageSize': '100',
      },
    });
  }

  // ── Images ──────────────────────────────────────────────────────────────

  /**
   * Uploads directly to Cloudinary for real XHR progress, then confirms with
   * the backend. Progress events: 0–90% = actual upload, 90% = backend saving,
   * 100% = done.
   */
  uploadImage(packageId: number, file: File): Observable<ImageUploadProgress> {
    // Compute SHA-256 of the compressed file (for duplicate detection on backend)
    const hashPromise = file.arrayBuffer().then((buf) =>
      crypto.subtle.digest('SHA-256', buf).then((hashBuf) =>
        [...new Uint8Array(hashBuf)].map((b) => b.toString(16).padStart(2, '0')).join(''),
      ),
    );

    return from(hashPromise).pipe(
      switchMap((fileHash) =>
        this.http.get<CloudinaryUploadParams>(`${this.baseUrl}/${packageId}/images/upload-params`).pipe(
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
                // Cloudinary responded — confirm with backend (brief processing state)
                const body = (e as any).body as CloudinaryUploadResponse;
                return concat(
                  of<ImageUploadProgress>({ progress: 90, done: false }),
                  this.http
                    .post<ServicePackageImageDto>(`${this.baseUrl}/${packageId}/images/confirm`, {
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

  deleteImage(packageId: number, imageId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${packageId}/images/${imageId}`);
  }

  setThumbnail(packageId: number, imageId: number): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${packageId}/images/${imageId}/thumbnail`, {});
  }

  getDownloadUrl(packageId: number, imageId: number): string {
    return `${this.baseUrl}/${packageId}/images/${imageId}/download`;
  }
}
