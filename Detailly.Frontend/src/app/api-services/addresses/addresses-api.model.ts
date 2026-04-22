import { PageResult } from '../../core/models/paging/page-result';

export interface ListMyAddressesQueryDto {
  id: number;
  street: string;
  city: string;
  postalCode: string;
  region?: string | null;
  country: string;
}

export interface CreateAddressCommand {
  street: string;
  city: string;
  postalCode: string;
  region?: string | null;
  country: string;
}

export interface UpdateAddressCommand {
  street?: string | null;
  city?: string | null;
  postalCode?: string | null;
  region?: string | null;
  country?: string | null;
}

export type ListMyAddressesResponse = PageResult<ListMyAddressesQueryDto>;
