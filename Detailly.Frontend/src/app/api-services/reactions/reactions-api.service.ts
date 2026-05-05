import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MyReactionDto, ReactionSummaryDto, ReactionType } from './reactions-api.models';

@Injectable({ providedIn: 'root' })
export class ReactionsApiService {
  private baseUrl = `${environment.apiUrl}/Reactions`;

  constructor(private http: HttpClient) {}

  upsert(servicePackageId: number, reactionType: ReactionType): Observable<ReactionSummaryDto> {
    return this.http.put<ReactionSummaryDto>(
      `${this.baseUrl}/service-packages/${servicePackageId}`,
      { reactionType },
    );
  }

  getMy(): Observable<MyReactionDto[]> {
    return this.http.get<MyReactionDto[]>(`${this.baseUrl}/service-packages/my`);
  }
}
