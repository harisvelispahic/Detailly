import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface WalletDto {
  balance: number;
  currency: string;
  totalDeposited: number;
  percentageAdded: number;
}

@Injectable({ providedIn: 'root' })
export class PaymentsService {
  private baseUrl = `${environment.apiUrl}/Payments`;

  constructor(private http: HttpClient) {}

  getMyWallet(): Observable<WalletDto> {
    return this.http.get<WalletDto>(`${this.baseUrl}/wallet/my`);
  }

  // -----------------------------
  // BOOKINGS
  // -----------------------------
  createCardIntent(bookingId: number): Observable<{ clientSecret: string }> {
    return this.http.post<{ clientSecret: string }>(
      `${this.baseUrl}/bookings/card-intent/${bookingId}`,
      {},
    );
  }

  payBookingWithWallet(bookingId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/bookings/wallet/${bookingId}`, {});
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
