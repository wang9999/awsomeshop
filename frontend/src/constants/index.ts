// Application constants

// Product categories
export const PRODUCT_CATEGORIES = [
  { label: '电子产品', value: '电子产品' },
  { label: '生活用品', value: '生活用品' },
  { label: '礼品卡', value: '礼品卡' },
  { label: '图书文具', value: '图书文具' },
];

// Order status
export const ORDER_STATUS = [
  { label: '待确认', value: '待确认', color: 'default' },
  { label: '待发货', value: '待发货', color: 'processing' },
  { label: '已发货', value: '已发货', color: 'blue' },
  { label: '已完成', value: '已完成', color: 'success' },
  { label: '已取消', value: '已取消', color: 'error' },
];

// Admin roles
export const ADMIN_ROLES = [
  { label: '超级管理员', value: 'SuperAdmin' },
  { label: '产品管理员', value: 'ProductAdmin' },
  { label: '积分管理员', value: 'PointsAdmin' },
];

// Points transaction types
export const POINTS_TRANSACTION_TYPES = [
  { label: '发放', value: '发放', color: 'success' },
  { label: '扣除', value: '扣除', color: 'warning' },
  { label: '消费', value: '消费', color: 'default' },
  { label: '退回', value: '退回', color: 'processing' },
  { label: '过期', value: '过期', color: 'error' },
];

// Pagination defaults
export const DEFAULT_PAGE_SIZE = 20;
export const PAGE_SIZE_OPTIONS = ['10', '20', '50', '100'];

// Session timeout (in milliseconds)
export const SESSION_TIMEOUT = 2 * 60 * 60 * 1000; // 2 hours
export const IDLE_TIMEOUT = 30 * 60 * 1000; // 30 minutes

// Monthly redemption limit
export const MONTHLY_REDEMPTION_LIMIT = 5;
