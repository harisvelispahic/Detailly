import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateEmployeeShiftCommand,
  ListEmployeeShiftsRequest,
  ListEmployeeShiftsResponse,
  MyShiftDto,
  UpdateEmployeeShiftCommand,
} from './employee-shifts-api.models';
import { buildHttpParams } from '../../core/models/build-http-params';

@Injectable({ providedIn: 'root' })
export class EmployeeShiftsApiService {
  private readonly baseUrl = `${environment.apiUrl}/EmployeeShifts`;
  private http = inject(HttpClient);

  list(request: ListEmployeeShiftsRequest): Observable<ListEmployeeShiftsResponse> {
    const params = buildHttpParams(request as any);
    return this.http.get<ListEmployeeShiftsResponse>(this.baseUrl, { params });
  }

  create(command: CreateEmployeeShiftCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateEmployeeShiftCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  listMine(days: number = 7): Observable<MyShiftDto[]> {
    const params = buildHttpParams({ days });
    return this.http.get<MyShiftDto[]>(`${this.baseUrl}/mine`, { params });
  }
}
