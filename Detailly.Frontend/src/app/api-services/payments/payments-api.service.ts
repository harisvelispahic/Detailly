import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PaymentsService {
  private baseUrl = `${environment.apiUrl}/Payments`;

  constructor(private http: HttpClient) {}

  // -----------------------------
  // BOOKINGS
  // -----------------------------
  createCardIntent(bookingId: number): Observable<{ clientSecret: string }> {
    return this.http.post<{ clientSecret: string }>(
      `${this.baseUrl}/bookings/card-intent/${bookingId}`,
      {},
    );
  }

  // -----------------------------
  // ORDERS
  // -----------------------------
  createOrderCardIntent(orderId: number): Observable<{ clientSecret: string }> {
    return this.http.post<{ clientSecret: string }>(
      `${this.baseUrl}/orders/card-intent/${orderId}`,
      {},
    );
  }

  // -----------------------------
  // WALLET TOP-UP
  // -----------------------------
  createWalletTopUpCardIntent(
    body: CreateWalletTopUpIntentRequest,
  ): Observable<{ clientSecret: string }> {
    return this.http.post<{ clientSecret: string }>(
      `${this.baseUrl}/wallet/top-up/card-intent`,
      body,
    );
  }
}

export interface CreateWalletTopUpIntentRequest {
  amount: number;
  description?: string | null;
}
