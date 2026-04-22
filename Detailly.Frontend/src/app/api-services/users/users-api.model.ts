export interface CreateUserCommand {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  phone?: string | null;
  isFleet: boolean;
  companyName?: string | null;
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
  companyName?: string | null;
}

export interface UpdateUserCommand {
  firstName?: string | null;
  lastName?: string | null;
  username?: string | null;
  email?: string | null;
  phone?: string | null;
  companyName?: string | null;
}
