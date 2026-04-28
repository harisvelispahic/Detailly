import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export enum EmployeeWorkMode {
  InShop = 0,
  Mobile = 1,
}

export class ListEmployeeShiftsRequest extends BasePagedQuery {
  dateUtc: string = '';
  shopLocationId: number = 0;
  employeeWorkMode?: EmployeeWorkMode | null;

  constructor() {
    super();
    this.paging.pageSize = 100;
  }
}

export interface EmployeeShiftDto {
  id: number;
  employeeId: number;
  employeeName: string;
  shopLocationId: number;
  employeeWorkMode: EmployeeWorkMode;
  startUtc: string;
  endUtc: string;
}

export type ListEmployeeShiftsResponse = PageResult<EmployeeShiftDto>;

export interface CreateEmployeeShiftCommand {
  employeeId: number;
  shopLocationId: number;
  employeeWorkMode: EmployeeWorkMode;
  startUtc: string;
  endUtc: string;
}

export interface UpdateEmployeeShiftCommand {
  employeeId?: number | null;
  shopLocationId?: number | null;
  employeeWorkMode?: EmployeeWorkMode | null;
  startUtc?: string | null;
  endUtc?: string | null;
}

export interface MyShiftDto {
  id: number;
  shopLocationId: number;
  shopLocationName: string;
  employeeWorkMode: EmployeeWorkMode;
  startUtc: string;
  endUtc: string;
}
