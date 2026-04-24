import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CancelBookingCommand,
  GetBookingByIdQueryDto,
  ListMyBookingsQueryDto,
  ListMyBookingsRequest,
} from './bookings-api.models';
import { PageResult } from '../../core/models/paging/page-result';
import { buildHttpParams } from '../../core/models/build-http-params';

@Injectable({ providedIn: 'root' })
export class BookingsService {
  private baseUrl = `${environment.apiUrl}/Bookings`;

  constructor(private http: HttpClient) {}

  listMine(request?: ListMyBookingsRequest): Observable<PageResult<ListMyBookingsQueryDto>> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<PageResult<ListMyBookingsQueryDto>>(`${this.baseUrl}/my`, { params });
  }

  getById(id: number): Observable<GetBookingByIdQueryDto> {
    return this.http.get<GetBookingByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  cancelBooking(bookingId: number, body?: CancelBookingCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/cancel/${bookingId}`, body ?? {});
  }
}
