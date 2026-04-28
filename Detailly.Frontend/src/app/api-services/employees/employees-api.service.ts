import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListEmployeesRequest, ListEmployeesResponse } from './employees-api.models';
import { buildHttpParams } from '../../core/models/build-http-params';

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Employees`;
  private http = inject(HttpClient);

  list(request?: ListEmployeesRequest): Observable<ListEmployeesResponse> {
    const params = buildHttpParams((request ?? new ListEmployeesRequest()) as any);
    return this.http.get<ListEmployeesResponse>(this.baseUrl, { params });
  }
}
