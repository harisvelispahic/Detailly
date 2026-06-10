import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export interface CreateUserCommand {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  phone?: string | null;
}

export interface CreateUserCommandDto {
  id: number;
}

export interface GetUserByIdQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  phone?: string | null;
  companyName?: string | null;
  isOAuthUser: boolean;
}

export interface UpdateUserCommand {
  firstName?: string | null;
  lastName?: string | null;
  username?: string | null;
  email?: string | null;
  phone?: string | null;
  companyName?: string | null;
}

export interface ChangePasswordCommand {
  currentPassword: string;
  newPassword: string;
}

export interface ListUsersQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  companyName?: string | null;
  isFleet: boolean;
}

export interface SetFleetStatusCommand {
  isFleet: boolean;
}

export class ListUsersRequest extends BasePagedQuery {
  search?: string | null;

  constructor() {
    super();
    this.paging.pageSize = 20;
  }
}

export type ListUsersResponse = PageResult<ListUsersQueryDto>;
