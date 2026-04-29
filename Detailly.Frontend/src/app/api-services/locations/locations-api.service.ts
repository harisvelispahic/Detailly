import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateLocationCommand,
  GetLocationByIdDto,
  ListLocationsRequest,
  ListLocationsResponse,
  LocationOpeningHoursDto,
  UpdateLocationCommand,
} from './locations-api.models';
import { buildHttpParams } from '../../core/models/build-http-params';

@Injectable({ providedIn: 'root' })
export class LocationsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Locations`;
  private http = inject(HttpClient);

  list(request?: ListLocationsRequest): Observable<ListLocationsResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListLocationsResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetLocationByIdDto> {
    return this.http.get<GetLocationByIdDto>(`${this.baseUrl}/${id}`);
  }

  create(command: CreateLocationCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateLocationCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getOpeningHours(locationId: number): Observable<LocationOpeningHoursDto[]> {
    return this.http.get<LocationOpeningHoursDto[]>(`${this.baseUrl}/${locationId}/hours`);
  }
}
