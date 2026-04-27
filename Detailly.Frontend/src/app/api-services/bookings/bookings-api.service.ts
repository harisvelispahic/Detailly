import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AvailabilitySlotDto,
  CancelBookingCommand,
  CreateBookingHoldCommand,
  GetAvailabilityRequest,
  GetAvailabilityResponse,
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

  createHold(command: CreateBookingHoldCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, command);
  }

  getAvailability(request: GetAvailabilityRequest): Observable<GetAvailabilityResponse> {
    let params = new HttpParams()
      .set('dateUtc', request.dateUtc)
      .set('servicePackageId', request.servicePackageId.toString())
      .set('serviceMode', request.serviceMode.toString())
      .set('shopLocationId', request.shopLocationId.toString());

    if (request.addonItemIds && request.addonItemIds.length > 0) {
      request.addonItemIds.forEach(id => {
        params = params.append('addonItemIds', id.toString());
      });
    }

    if (request.serviceAddressId != null) {
      params = params.set('serviceAddressId', request.serviceAddressId.toString());
    }

    return this.http.get<GetAvailabilityResponse>(`${this.baseUrl}/availability`, { params });
  }
}
