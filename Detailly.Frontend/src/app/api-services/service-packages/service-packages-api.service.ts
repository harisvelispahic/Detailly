import { HttpClient, HttpEvent, HttpEventType, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, filter, map } from 'rxjs';
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
  progress: number;        // 0–100
  done: boolean;
  result?: ServicePackageImageDto;
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

  uploadImage(packageId: number, file: File): Observable<ImageUploadProgress> {
    const formData = new FormData();
    formData.append('file', file);

    const req = new HttpRequest('POST', `${this.baseUrl}/${packageId}/images`, formData, {
      reportProgress: true,
    });

    return this.http.request<ServicePackageImageDto>(req).pipe(
      filter(
        (e): e is HttpEvent<ServicePackageImageDto> =>
          e.type === HttpEventType.UploadProgress || e.type === HttpEventType.Response,
      ),
      map((e) => {
        if (e.type === HttpEventType.UploadProgress) {
          const total = e.total ?? e.loaded;
          // Cap at 80 — the remaining 20% represents server-side Cloudinary processing
          return { progress: Math.round((80 * e.loaded) / total), done: false };
        }
        return { progress: 100, done: true, result: (e as any).body as ServicePackageImageDto };
      }),
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
