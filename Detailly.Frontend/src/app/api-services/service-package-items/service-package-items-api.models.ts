import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export interface ListServicePackageItemsQueryDto {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  durationMinutes: number;
  requiredEmployees: number;
  isAddon: boolean;
  isActive: boolean;
}

export interface GetServicePackageItemByIdQueryDto {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  durationMinutes: number;
  requiredEmployees: number;
  isAddon: boolean;
  isActive: boolean;
}

export interface CreateServicePackageItemCommand {
  name: string;
  description?: string | null;
  price: number;
  durationMinutes: number;
  requiredEmployees: number;
  isAddon: boolean;
}

export interface UpdateServicePackageItemCommand {
  name?: string | null;
  description?: string | null;
  price?: number | null;
  durationMinutes?: number | null;
  requiredEmployees?: number | null;
  isAddon?: boolean | null;
  isActive?: boolean | null;
}

export class ListServicePackageItemsRequest extends BasePagedQuery {
  search?: string | null;
  includeInactive?: boolean;

  constructor() {
    super();
    this.paging.pageSize = 200;
  }
}

export type ListServicePackageItemsResponse = PageResult<ListServicePackageItemsQueryDto>;
