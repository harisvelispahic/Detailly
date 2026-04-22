import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateAddressCommand,
  ListMyAddressesResponse,
  UpdateAddressCommand,
} from './addresses-api.model';

@Injectable({ providedIn: 'root' })
export class AddressesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Addresses`;
  private http = inject(HttpClient);

  listMine(): Observable<ListMyAddressesResponse> {
    return this.http.get<ListMyAddressesResponse>(`${this.baseUrl}/my`, {
      params: { 'paging.pageSize': '100' },
    });
  }

  create(command: CreateAddressCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  update(id: number, command: UpdateAddressCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
