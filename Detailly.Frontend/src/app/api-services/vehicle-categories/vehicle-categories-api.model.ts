import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export interface ListVehicleCategoriesQueryDto {
  id: number;
  name: string;
  description?: string | null;
  basePriceMultiplier: number;
}

export interface GetVehicleCategoryByIdQueryDto {
  id: number;
  name: string;
  description?: string | null;
  basePriceMultiplier: number;
}

export interface CreateVehicleCategoryCommand {
  name: string;
  description?: string | null;
  basePriceMultiplier: number;
}

export interface UpdateVehicleCategoryCommand {
  name?: string | null;
  description?: string | null;
  basePriceMultiplier?: number | null;
}

export class ListVehicleCategoriesRequest extends BasePagedQuery {
  search?: string | null;

  constructor() {
    super();
    this.paging.pageSize = 20;
  }
}

export type ListVehicleCategoriesResponse = PageResult<ListVehicleCategoriesQueryDto>;

// Legacy alias used elsewhere in the app (e.g. booking wizard)
export interface VehicleCategoryDto {
  id: number;
  name: string;
  description?: string | null;
  basePriceMultiplier: number;
}
