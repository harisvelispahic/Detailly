import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateReviewCommand,
  GetMyReviewForServicePackageDto,
  ListReviewsRequest,
  ListReviewsResponse,
} from './reviews-api.models';

@Injectable({ providedIn: 'root' })
export class ReviewsApiService {
  private baseUrl = `${environment.apiUrl}/Reviews`;

  constructor(private http: HttpClient) {}

  list(request: ListReviewsRequest): Observable<ListReviewsResponse> {
    const params = buildHttpParams(request as any);
    return this.http.get<ListReviewsResponse>(this.baseUrl, { params });
  }

  createOrUpdate(bookingId: number, command: CreateReviewCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.baseUrl}/${bookingId}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getMyReviewForServicePackage(servicePackageId: number): Observable<GetMyReviewForServicePackageDto | null> {
    return this.http.get<GetMyReviewForServicePackageDto | null>(
      `${this.baseUrl}/my/service-package/${servicePackageId}`
    );
  }
}
