import { get, post } from '../utils/request';

export interface PointsBalance {
  balance: number;
  expiringPoints: number;
}

export interface PointsTransaction {
  id: string;
  employeeId: string;
  employeeName: string;
  amount: number;
  type: string;
  reason?: string;
  operatorId?: string;
  operatorType: string;
  balanceBefore: number;
  balanceAfter: number;
  relatedOrderId?: string;
  expiryDate?: string;
  createdAt: string;
}

export interface PointsTransactionListResponse {
  transactions: PointsTransaction[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface GrantPointsRequest {
  employeeId: string;
  amount: number;
  reason: string;
  expiryDate?: string;
}

export interface BatchGrantPointsRequest {
  employeeIds: string[];
  amount: number;
  reason: string;
  expiryDate?: string;
}

export interface DeductPointsRequest {
  employeeId: string;
  amount: number;
  reason: string;
}

/**
 * Get current employee's points balance
 */
export const getPointsBalance = (): Promise<PointsBalance> => {
  return get<PointsBalance>('/points/balance');
};

/**
 * Get current employee's points transaction history
 */
export const getPointsTransactions = (params: {
  pageNumber?: number;
  pageSize?: number;
  startDate?: string;
  endDate?: string;
}): Promise<PointsTransactionListResponse> => {
  return get<PointsTransactionListResponse>('/points/transactions', params);
};

/**
 * Export points transaction history as CSV
 */
export const exportPointsTransactions = (params: {
  startDate?: string;
  endDate?: string;
}): Promise<Blob> => {
  return get<Blob>('/points/transactions/export', params, { responseType: 'blob' });
};

/**
 * Admin: Grant points to a single employee
 */
export const grantPoints = (data: GrantPointsRequest): Promise<{ message: string }> => {
  return post<{ message: string }>('/admin/points/grant', data);
};

/**
 * Admin: Grant points to multiple employees
 */
export const batchGrantPoints = (data: BatchGrantPointsRequest): Promise<{
  message: string;
  successCount: number;
  totalCount: number;
  failedEmployees: string[];
}> => {
  return post('/admin/points/grant/batch', data);
};

/**
 * Admin: Deduct points from an employee
 */
export const deductPoints = (data: DeductPointsRequest): Promise<{ message: string }> => {
  return post<{ message: string }>('/admin/points/deduct', data);
};
