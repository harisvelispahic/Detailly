import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CancelBookingCommand,
  GetBookingByIdQueryDto,
  ListMyBookingsQueryDto,
} from './bookings-api.models';

@Injectable({ providedIn: 'root' })
export class BookingsService {
  private baseUrl = `${environment.apiUrl}/Bookings`;

  constructor(private http: HttpClient) {}

  listMine(): Observable<ListMyBookingsQueryDto[]> {
    return this.http.get<ListMyBookingsQueryDto[]>(this.baseUrl);
  }

  getById(id: number): Observable<GetBookingByIdQueryDto> {
    return this.http.get<GetBookingByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  cancelBooking(bookingId: number, body?: CancelBookingCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/cancel/${bookingId}`, body ?? {});
  }
}
