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
