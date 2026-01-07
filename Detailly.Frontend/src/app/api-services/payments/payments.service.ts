import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PaymentsService {
  private baseUrl = `${environment.apiUrl}/api/payments`;

  constructor(private http: HttpClient) {}

  createCardIntent(bookingId: number): Observable<{ clientSecret: string }> {
    return this.http.post<{ clientSecret: string }>(
      `${this.baseUrl}/bookings/${bookingId}/card-intent`,
      {}
    );
  }
}
