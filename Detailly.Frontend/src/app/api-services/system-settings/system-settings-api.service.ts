import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SystemSettingsDto, UpdateSystemSettingsCommand } from './system-settings-api.model';

@Injectable({
  providedIn: 'root',
})
export class SystemSettingsApiService {
  private readonly baseUrl = `${environment.apiUrl}/SystemSettings`;
  private http = inject(HttpClient);

  get(): Observable<SystemSettingsDto> {
    return this.http.get<SystemSettingsDto>(this.baseUrl);
  }

  update(payload: UpdateSystemSettingsCommand): Observable<void> {
    return this.http.put<void>(this.baseUrl, payload);
  }
}
