import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeeDto } from './employees-api.models';

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Employees`;
  private http = inject(HttpClient);

  list(): Observable<EmployeeDto[]> {
    return this.http.get<EmployeeDto[]>(this.baseUrl);
  }
}
