import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateServicePackageItemCommand,
  GetServicePackageItemByIdQueryDto,
  ListServicePackageItemsRequest,
  ListServicePackageItemsResponse,
  UpdateServicePackageItemCommand,
} from './service-package-items-api.models';

@Injectable({ providedIn: 'root' })
export class ServicePackageItemsApiService {
  private readonly baseUrl = `${environment.apiUrl}/ServicePackageItems`;
  private http = inject(HttpClient);

  list(request?: ListServicePackageItemsRequest): Observable<ListServicePackageItemsResponse> {
    const params = buildHttpParams((request ?? new ListServicePackageItemsRequest()) as any);
    return this.http.get<ListServicePackageItemsResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetServicePackageItemByIdQueryDto> {
    return this.http.get<GetServicePackageItemByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(command: CreateServicePackageItemCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateServicePackageItemCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
