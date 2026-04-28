import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export interface CreateStaffMemberCommand {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  phone?: string | null;
  isManager: boolean;
}

export interface UpdateStaffMemberCommand {
  firstName?: string | null;
  lastName?: string | null;
  username?: string | null;
  email?: string | null;
  phone?: string | null;
}

export interface GetStaffMemberByIdQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  phone?: string | null;
  isManager: boolean;
}

export interface ListStaffMembersQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  phone?: string | null;
  isManager: boolean;
}

export class ListStaffMembersRequest extends BasePagedQuery {
  search?: string | null;
  isManager?: boolean | null;

  constructor() {
    super();
    this.paging.pageSize = 20;
  }
}

export type ListStaffMembersResponse = PageResult<ListStaffMembersQueryDto>;
