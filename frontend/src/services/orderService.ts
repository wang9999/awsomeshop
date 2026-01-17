import { get, post, put } from '../utils/request';

export interface Order {
  id: string;
  orderNumber: string;
  items: OrderItem[];
  totalPoints: number;
  status: string;
  trackingNumber?: string;
  address?: Address;
  createdAt: string;
}

export interface OrderItem {
  productId: string;
  productName: string;
  productImage?: string;
  points: number;
  quantity: number;
}

export interface Address {
  id: string;
  receiverName: string;
  receiverPhone: string;
  province?: string;
  city?: string;
  district?: string;
  detailAddress: string;
  isDefault: boolean;
}

export interface CreateOrderRequest {
  items: { productId: string; quantity: number }[];
  addressId: string;
  captcha: string;
}

export interface OrderListResponse {
  orders: Order[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface OrderQueryParams {
  page?: number;
  pageSize?: number;
  status?: string;
  startDate?: string;
  endDate?: string;
}

/**
 * Create order
 */
export const createOrder = (data: CreateOrderRequest): Promise<Order> => {
  return post<Order>('/orders', data);
};

/**
 * Get order by ID
 */
export const getOrderById = (id: string): Promise<Order> => {
  return get<Order>(`/orders/${id}`);
};

/**
 * Get my orders
 */
export const getMyOrders = (params?: OrderQueryParams): Promise<OrderListResponse> => {
  return get<OrderListResponse>('/orders', params);
};

/**
 * Cancel order
 */
export const cancelOrder = (id: string, reason?: string): Promise<Order> => {
  return put<Order>(`/orders/${id}/cancel`, { reason });
};

/**
 * Admin: Get all orders
 */
export const getAllOrders = (params?: OrderQueryParams & { employeeId?: string }): Promise<OrderListResponse> => {
  return get<OrderListResponse>('/admin/orders', params);
};

/**
 * Admin: Update order status
 */
export const updateOrderStatus = (id: string, status: string, trackingNumber?: string): Promise<Order> => {
  return put<Order>(`/admin/orders/${id}/status`, { status, trackingNumber });
};
