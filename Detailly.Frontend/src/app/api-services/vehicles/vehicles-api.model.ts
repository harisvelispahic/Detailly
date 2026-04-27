import { PageResult } from '../../core/models/paging/page-result';

export interface ListMyVehiclesQueryDto {
  id: number;
  brand: string;
  model: string;
  yearOfManufacture: number;
  licencePlate?: string | null;
  notes?: string | null;
  vehicleCategory: { name: string; basePriceMultiplier: number };
}

export interface CreateVehicleCommand {
  brand: string;
  model: string;
  yearOfManufacture: number;
  vehicleCategoryId: number;
  licencePlate: string;
  notes?: string | null;
}

export interface UpdateVehicleCommand {
  brand?: string | null;
  model?: string | null;
  yearOfManufacture?: number | null;
  licencePlate?: string | null;
  notes?: string | null;
  vehicleCategoryId?: number | null;
}

export type ListMyVehiclesResponse = PageResult<ListMyVehiclesQueryDto>;
