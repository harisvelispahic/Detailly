import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  LinkExternalAccountCommand,
  LinkExternalAccountCommandDto,
  LoginCommand,
  LoginCommandDto,
  LogoutCommand,
  RefreshTokenCommand,
  RefreshTokenCommandDto,
} from './auth-api.model';

@Injectable({
  providedIn: 'root',
})
export class AuthApiService {
  private readonly baseUrl = `${environment.apiUrl}/Auth`;
  private readonly externalBaseUrl = `${environment.apiUrl}/auth/external`;
  private http = inject(HttpClient);

  login(payload: LoginCommand): Observable<LoginCommandDto> {
    return this.http.post<LoginCommandDto>(`${this.baseUrl}/login`, payload);
  }

  refresh(payload: RefreshTokenCommand): Observable<RefreshTokenCommandDto> {
    return this.http.post<RefreshTokenCommandDto>(`${this.baseUrl}/refresh`, payload);
  }

  logout(payload: LogoutCommand): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, payload);
  }

  linkExternalAccount(payload: LinkExternalAccountCommand): Observable<LinkExternalAccountCommandDto> {
    return this.http.post<LinkExternalAccountCommandDto>(`${this.externalBaseUrl}/link`, payload);
  }
}
