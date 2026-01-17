import { get } from '../utils/request';

export interface Employee {
  id: string;
  email: string;
  name: string;
  employeeNumber: string;
  department?: string;
  pointsBalance: number;
  isActive: boolean;
}

export interface EmployeeListResponse {
  employees: Employee[];
  totalCount: number;
}

/**
 * Get employee list (Admin only)
 */
export const getEmployees = (params?: {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
}): Promise<EmployeeListResponse> => {
  return get<EmployeeListResponse>('/admin/employees', params);
};

/**
 * Get employee by ID (Admin only)
 */
export const getEmployeeById = (id: string): Promise<Employee> => {
  return get<Employee>(`/admin/employees/${id}`);
};
