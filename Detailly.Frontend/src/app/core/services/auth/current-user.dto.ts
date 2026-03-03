export interface CurrentUserDto {
  userId: number;
  email: string;

  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;

  isFleet: boolean;
  isStandard: boolean; // derived = !isFleet

  // derived convenience
  isStaff: boolean;
  isAdminOrManager: boolean;

  tokenVersion: number;
}
