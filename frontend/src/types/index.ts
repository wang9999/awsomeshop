// Shared TypeScript types

export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errorCode?: string;
}

export interface PaginationParams {
  page?: number;
  pageSize?: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

// Product categories
export type ProductCategory = '电子产品' | '生活用品' | '礼品卡' | '图书文具';

// Product status
export type ProductStatus = '上架' | '下架';

// Order status
export type OrderStatus = '待确认' | '待发货' | '已发货' | '已完成' | '已取消';

// Admin roles
export type AdminRole = 'SuperAdmin' | 'ProductAdmin' | 'PointsAdmin';

// Points transaction types
export type PointsTransactionType = '发放' | '扣除' | '消费' | '退回' | '过期';

// Notification types
export type NotificationType = '积分变动' | '订单状态' | '产品上新' | '系统通知';
