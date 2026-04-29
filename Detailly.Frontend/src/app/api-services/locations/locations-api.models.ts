import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

// ── Opening Hours ─────────────────────────────────────────────────────────────

export interface LocationOpeningHoursDto {
  dayOfWeek: number; // 0=Sunday … 6=Saturday
  isClosed: boolean;
  openHour: number | null;
  openMinute: number | null;
  closeHour: number | null;
  closeMinute: number | null;
}

export interface LocationOpeningHoursInputDto {
  dayOfWeek: number;
  isClosed: boolean;
  openHour: number | null;
  openMinute: number | null;
  closeHour: number | null;
  closeMinute: number | null;
}

// ── List ──────────────────────────────────────────────────────────────────────

export interface ListLocationsQueryDto {
  id: number;
  name: string;
  totalBays: number;
  street: string | null;
  city: string | null;
  postalCode: string | null;
  region: string | null;
  country: string | null;
  isOpenToday: boolean;
  isTemporarilyClosed: boolean;
}

export class ListLocationsRequest extends BasePagedQuery {
  search?: string | null;

  constructor() {
    super();
    this.paging.pageSize = 20;
  }
}

export type ListLocationsResponse = PageResult<ListLocationsQueryDto>;

// Backward-compat alias used by the shifts module
export type LocationDto = ListLocationsQueryDto;

// ── GetById ───────────────────────────────────────────────────────────────────

export interface GetLocationByIdDto {
  id: number;
  name: string;
  description: string | null;
  totalBays: number;
  addressId: number;
  address: LocationAddressDto;
}

export interface LocationAddressDto {
  id: number;
  street: string | null;
  city: string | null;
  postalCode: string | null;
  region: string | null;
  country: string | null;
  latitude: number | null;
  longitude: number | null;
}

// ── Commands ──────────────────────────────────────────────────────────────────

export interface CreateLocationAddressDto {
  street: string;
  city: string;
  postalCode: string;
  region?: string | null;
  country: string;
}

export interface CreateLocationCommand {
  name: string;
  description?: string | null;
  totalBays: number;
  address: CreateLocationAddressDto;
  openingHours?: LocationOpeningHoursInputDto[] | null;
}

export interface UpdateLocationAddressDto {
  street?: string | null;
  city?: string | null;
  postalCode?: string | null;
  region?: string | null;
  country?: string | null;
}

export interface UpdateLocationCommand {
  name?: string | null;
  description?: string | null;
  totalBays?: number | null;
  address?: UpdateLocationAddressDto | null;
  openingHours?: LocationOpeningHoursInputDto[] | null;
}
