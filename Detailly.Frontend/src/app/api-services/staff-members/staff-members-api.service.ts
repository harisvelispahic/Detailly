import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateStaffMemberCommand,
  EmployeeDto,
  GetStaffMemberByIdQueryDto,
  ListAvailableEmployeesForShiftRequest,
  ListAvailableEmployeesForShiftResponse,
  ListStaffMembersRequest,
  ListStaffMembersResponse,
  UpdateStaffMemberCommand,
} from './staff-members-api.models';

@Injectable({ providedIn: 'root' })
export class StaffMembersApiService {
  private readonly baseUrl = `${environment.apiUrl}/Staff`;
  private http = inject(HttpClient);

  list(request?: ListStaffMembersRequest): Observable<ListStaffMembersResponse> {
    const params = buildHttpParams((request ?? new ListStaffMembersRequest()) as any);
    return this.http.get<ListStaffMembersResponse>(this.baseUrl, { params });
  }

  listAvailableForShift(
    request?: ListAvailableEmployeesForShiftRequest,
  ): Observable<ListAvailableEmployeesForShiftResponse> {
    const params = buildHttpParams((request ?? new ListAvailableEmployeesForShiftRequest()) as any);
    return this.http.get<ListAvailableEmployeesForShiftResponse>(`${this.baseUrl}/employees`, {
      params,
    });
  }

  getById(id: number): Observable<GetStaffMemberByIdQueryDto> {
    return this.http.get<GetStaffMemberByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(command: CreateStaffMemberCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateStaffMemberCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
