import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeeDto } from './employees-api.models';
import { EmployeeWorkMode } from '../employee-shifts/employee-shifts-api.models';

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Employees`;
  private http = inject(HttpClient);

  list(workMode?: EmployeeWorkMode | null): Observable<EmployeeDto[]> {
    let params = new HttpParams();
    if (workMode != null) {
      params = params.set('employeeWorkMode', String(workMode));
    }
    return this.http.get<EmployeeDto[]>(this.baseUrl, { params });
  }
}
