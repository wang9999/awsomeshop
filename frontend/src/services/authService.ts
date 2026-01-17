import { post } from '../utils/request';

export interface LoginRequest {
  email?: string;
  username?: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  employeeId?: string;
  adminId?: string;
  name: string;
  email?: string;
  role?: string;
  points?: number;
}

/**
 * Employee login
 */
export const employeeLogin = (data: LoginRequest): Promise<LoginResponse> => {
  return post<LoginResponse>('/auth/employee/login', data);
};

/**
 * Admin login
 */
export const adminLogin = (data: LoginRequest): Promise<LoginResponse> => {
  return post<LoginResponse>('/auth/admin/login', data);
};

/**
 * Get current employee info
 */
export const getCurrentEmployee = (): Promise<any> => {
  return post('/employees/me');
};
