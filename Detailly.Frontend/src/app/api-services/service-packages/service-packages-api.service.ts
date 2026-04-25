import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  GetAvailableAddonsResponse,
  ListServicePackagesResponse,
} from './service-packages-api.models';

@Injectable({ providedIn: 'root' })
export class ServicePackagesApiService {
  private readonly baseUrl = `${environment.apiUrl}/ServicePackages`;
  private http = inject(HttpClient);

  list(): Observable<ListServicePackagesResponse> {
    return this.http.get<ListServicePackagesResponse>(this.baseUrl, {
      params: { 'paging.pageSize': '100' },
    });
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
