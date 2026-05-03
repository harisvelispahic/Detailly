import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateServicePackageCommand,
  GetAvailableAddonsResponse,
  GetServicePackageByIdQueryDto,
  ListServicePackagesRequest,
  ListServicePackagesResponse,
  UpdateServicePackageCommand,
} from './service-packages-api.models';

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
}
