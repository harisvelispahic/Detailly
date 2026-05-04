import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { map } from 'rxjs/operators';
import {
  CreateVehicleCategoryCommand,
  GetVehicleCategoryByIdQueryDto,
  ListVehicleCategoriesQueryDto,
  ListVehicleCategoriesRequest,
  ListVehicleCategoriesResponse,
  UpdateVehicleCategoryCommand,
} from './vehicle-categories-api.model';

@Injectable({ providedIn: 'root' })
export class VehicleCategoriesApiService {
  private readonly baseUrl = `${environment.apiUrl}/VehicleCategories`;
  private http = inject(HttpClient);

  list(request?: ListVehicleCategoriesRequest): Observable<ListVehicleCategoriesResponse> {
    const params = buildHttpParams((request ?? new ListVehicleCategoriesRequest()) as any);
    return this.http.get<ListVehicleCategoriesResponse>(this.baseUrl, { params });
  }

  listAll(): Observable<ListVehicleCategoriesQueryDto[]> {
    const req = new ListVehicleCategoriesRequest();
    req.paging.pageSize = 100;
    const params = buildHttpParams(req as any);
    return this.http
      .get<ListVehicleCategoriesResponse>(this.baseUrl, { params })
      .pipe(map((r) => r.items as ListVehicleCategoriesQueryDto[]));
  }

  getById(id: number): Observable<GetVehicleCategoryByIdQueryDto> {
    return this.http.get<GetVehicleCategoryByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(command: CreateVehicleCategoryCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateVehicleCategoryCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
