import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export interface ServicePackageItemDto {
  id: number;
  name: string;
  price: number;
  description?: string | null;
}

export interface ListServicePackagesQueryDto {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  estimatedDurationMinutes: number;
  averageRating?: number | null;
  reviewCount: number;
  items: ServicePackageItemDto[];
}

export interface GetServicePackageByIdQueryDto {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  estimatedDurationHours: number;
  items: ServicePackageItemDto[];
}

export interface CreateServicePackageCommand {
  name: string;
  description?: string | null;
  price: number;
  servicePackageItemIds?: number[] | null;
}

export interface UpdateServicePackageCommand {
  name?: string | null;
  description?: string | null;
  price?: number | null;
  servicePackageItemIds?: number[] | null;
}

export class ListServicePackagesRequest extends BasePagedQuery {
  search?: string | null;

  constructor() {
    super();
    this.paging.pageSize = 100;
  }
}

export interface GetAvailableAddonsQueryDto {
  id: number;
  name: string;
  price: number;
  durationMinutes: number;
  requiredEmployees: number;
  description?: string | null;
}

export type ListServicePackagesResponse = PageResult<ListServicePackagesQueryDto>;
export type GetAvailableAddonsResponse = PageResult<GetAvailableAddonsQueryDto>;
