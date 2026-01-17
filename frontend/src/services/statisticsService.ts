import { get } from '../utils/request';

export interface OverviewStatistics {
  totalEmployees: number;
  totalProducts: number;
  totalOrders: number;
  totalPointsIssued: number;
  totalPointsConsumed: number;
  activeEmployees: number;
  onlineProducts: number;
  pendingOrders: number;
}

export interface ProductStatistics {
  productId: string;
  productName: string;
  orderCount: number;
  totalQuantity: number;
  totalPoints: number;
}

export interface PointsStatistics {
  date: string;
  pointsIssued: number;
  pointsConsumed: number;
  netChange: number;
}

export interface EmployeeStatistics {
  employeeId: string;
  employeeName: string;
  email: string;
  orderCount: number;
  totalPointsConsumed: number;
  lastOrderDate?: string;
}

export interface StatisticsQuery {
  startDate?: string;
  endDate?: string;
  topN?: number;
}

/**
 * Admin: Get overview statistics
 */
export const getOverviewStatistics = (params?: StatisticsQuery): Promise<OverviewStatistics> => {
  return get<OverviewStatistics>('/admin/statistics/overview', params);
};

/**
 * Admin: Get top products statistics
 */
export const getTopProducts = (params?: StatisticsQuery): Promise<ProductStatistics[]> => {
  return get<ProductStatistics[]>('/admin/statistics/products', params);
};

/**
 * Admin: Get points trend statistics
 */
export const getPointsTrend = (params?: StatisticsQuery): Promise<PointsStatistics[]> => {
  return get<PointsStatistics[]>('/admin/statistics/points', params);
};

/**
 * Admin: Get active employees statistics
 */
export const getActiveEmployees = (params?: StatisticsQuery): Promise<EmployeeStatistics[]> => {
  return get<EmployeeStatistics[]>('/admin/statistics/employees', params);
};
