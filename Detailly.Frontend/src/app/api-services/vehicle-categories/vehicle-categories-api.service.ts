import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { VehicleCategoryDto } from './vehicle-categories-api.model';

@Injectable({ providedIn: 'root' })
export class VehicleCategoriesApiService {
  private readonly baseUrl = `${environment.apiUrl}/VehicleCategories`;
  private http = inject(HttpClient);

  list(): Observable<VehicleCategoryDto[]> {
    return this.http.get<VehicleCategoryDto[]>(this.baseUrl);
  }
}
