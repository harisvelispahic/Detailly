import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export interface EmployeeDto {
  id: number;
  fullName: string;
}

export class ListEmployeesRequest extends BasePagedQuery {
  dateUtc?: string | null;
  excludeShiftId?: number | null;

  constructor() {
    super();
    this.paging.pageSize = 1000;
  }
}

export type ListEmployeesResponse = PageResult<EmployeeDto>;
