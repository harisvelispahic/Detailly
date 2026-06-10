import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { ChangePasswordCommand, CreateUserCommand, CreateUserCommandDto, GetUserByIdQueryDto, ListUsersRequest, ListUsersResponse, SetFleetStatusCommand, UpdateUserCommand } from './users-api.model';

@Injectable({ providedIn: 'root' })
export class UsersApiService {
  private readonly baseUrl = `${environment.apiUrl}/Users`;
  private http = inject(HttpClient);

  create(command: CreateUserCommand): Observable<CreateUserCommandDto> {
    return this.http.post<CreateUserCommandDto>(this.baseUrl, command);
  }

  getById(id: number): Observable<GetUserByIdQueryDto> {
    return this.http.get<GetUserByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  update(id: number, command: UpdateUserCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  changePassword(command: ChangePasswordCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/change-password`, command);
  }

  list(request?: ListUsersRequest): Observable<ListUsersResponse> {
    const params = buildHttpParams((request ?? new ListUsersRequest()) as any);
    return this.http.get<ListUsersResponse>(this.baseUrl, { params });
  }

  setFleetStatus(id: number, command: SetFleetStatusCommand): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/fleet-status`, command);
  }
}
