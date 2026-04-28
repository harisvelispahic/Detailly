import { EmployeeWorkMode } from '../employee-shifts/employee-shifts-api.models';

export interface EmployeeDto {
  id: number;
  fullName: string;
  employeeWorkMode: EmployeeWorkMode | null;
}
