import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export interface LocationOpeningHoursDto {
  dayOfWeek: number; // 0=Sunday … 6=Saturday
  isClosed: boolean;
  openHour: number | null;   // UTC hour (0-23)
  openMinute: number | null;
  closeHour: number | null;
  closeMinute: number | null;
}

export interface LocationDto {
  id: number;
  name: string;
  locationType: number;
  totalBays: number;
  city?: string | null;
  country?: string | null;
}

export class ListLocationsRequest extends BasePagedQuery {
  search?: string | null;

  constructor() {
    super();
    this.paging.pageSize = 100;
  }
}

export type ListLocationsResponse = PageResult<LocationDto>;
