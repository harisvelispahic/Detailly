import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateVehicleCommand,
  ListMyVehiclesResponse,
  UpdateVehicleCommand,
} from './vehicles-api.model';

@Injectable({ providedIn: 'root' })
export class VehiclesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Vehicles`;
  private http = inject(HttpClient);

  listMine(): Observable<ListMyVehiclesResponse> {
    return this.http.get<ListMyVehiclesResponse>(`${this.baseUrl}/my`, {
      params: { 'paging.pageSize': '100' },
    });
  }

  create(command: CreateVehicleCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateVehicleCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
