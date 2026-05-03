import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  ListServicePackageItemsRequest,
  ListServicePackageItemsResponse,
} from './service-package-items-api.models';

@Injectable({ providedIn: 'root' })
export class ServicePackageItemsApiService {
  private readonly baseUrl = `${environment.apiUrl}/ServicePackageItems`;
  private http = inject(HttpClient);

  list(request?: ListServicePackageItemsRequest): Observable<ListServicePackageItemsResponse> {
    const params = buildHttpParams((request ?? new ListServicePackageItemsRequest()) as any);
    return this.http.get<ListServicePackageItemsResponse>(this.baseUrl, { params });
  }
}
