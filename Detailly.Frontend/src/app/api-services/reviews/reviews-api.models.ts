import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListReviewsRequest extends BasePagedQuery {
  servicePackageId?: number;
  sort?: 'newest' | 'oldest' = 'newest';

  constructor() {
    super();
    this.paging.pageSize = 5;
  }
}

export interface ListReviewsQueryDto {
  id: number;
  servicePackageId: number;
  servicePackageName: string;
  customerFullName: string;
  customerInitials: string;
  rating: number;
  description?: string | null;
  addonNames: string[];
  createdAtUtc: string;
  modifiedAtUtc?: string | null;
  ratedAtUtc: string;
}

export class ListMyReviewsRequest extends BasePagedQuery {
  sort?: 'newest' | 'oldest' = 'newest';

  constructor() {
    super();
    this.paging.pageSize = 8;
  }
}

export interface ListMyReviewsQueryDto {
  id: number;
  servicePackageId: number;
  servicePackageName: string;
  rating: number;
  description?: string | null;
  addonNames: string[];
  createdAtUtc: string;
  modifiedAtUtc?: string | null;
  ratedAtUtc: string;
}

export type ListMyReviewsResponse = PageResult<ListMyReviewsQueryDto>;

export interface GetMyReviewForServicePackageDto {
  id: number;
  rating: number;
  description?: string | null;
  createdAtUtc: string;
}

export interface CreateReviewCommand {
  rating: number;
  description?: string | null;
}

export interface ReviewStats {
  averageRating: number;
  totalCount: number;
  distribution: { stars: number; count: number }[];
}

export type ListReviewsResponse = PageResult<ListReviewsQueryDto>;
