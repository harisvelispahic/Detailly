import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookingsService {
  private baseUrl = `${environment.apiUrl}/Bookings`;

  constructor(private http: HttpClient) {}

  cancelBooking(bookingId: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/cancel/${bookingId}`, {});
  }
}
