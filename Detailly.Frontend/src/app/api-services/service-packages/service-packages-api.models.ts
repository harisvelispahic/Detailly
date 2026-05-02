import { PageResult } from '../../core/models/paging/page-result';

export interface ListServicePackagesQueryDto {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  estimatedDurationHours: number;
  averageRating?: number | null;
  reviewCount: number;
  items: ServicePackageItemDto[];
}

export interface ServicePackageItemDto {
  id: number;
  name: string;
  price: number;
  description?: string | null;
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
