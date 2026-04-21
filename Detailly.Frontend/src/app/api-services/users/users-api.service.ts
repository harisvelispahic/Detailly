import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateUserCommand, CreateUserCommandDto } from './users-api.model';

@Injectable({ providedIn: 'root' })
export class UsersApiService {
  private readonly baseUrl = `${environment.apiUrl}/Users`;
  private http = inject(HttpClient);

  create(command: CreateUserCommand): Observable<CreateUserCommandDto> {
    return this.http.post<CreateUserCommandDto>(this.baseUrl, command);
  }
}
