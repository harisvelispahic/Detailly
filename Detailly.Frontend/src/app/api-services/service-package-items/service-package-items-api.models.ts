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

export class ListServicePackageItemsRequest extends BasePagedQuery {
  search?: string | null;

  constructor() {
    super();
    this.paging.pageSize = 200;
  }
}

export type ListServicePackageItemsResponse = PageResult<ListServicePackageItemsQueryDto>;
