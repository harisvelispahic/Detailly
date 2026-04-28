import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListLocationsRequest, ListLocationsResponse, LocationOpeningHoursDto } from './locations-api.models';
import { buildHttpParams } from '../../core/models/build-http-params';

@Injectable({ providedIn: 'root' })
export class LocationsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Locations`;
  private http = inject(HttpClient);

  list(request?: ListLocationsRequest): Observable<ListLocationsResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListLocationsResponse>(this.baseUrl, { params });
  }

  getOpeningHours(locationId: number): Observable<LocationOpeningHoursDto[]> {
    return this.http.get<LocationOpeningHoursDto[]>(`${this.baseUrl}/${locationId}/hours`);
  }
}
