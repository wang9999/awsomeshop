import { get, put } from '../utils/request';

export interface Notification {
  id: string;
  title: string;
  content: string;
  type: string;
  relatedId?: string;
  isRead: boolean;
  createdAt: string;
}

export interface NotificationListResponse {
  notifications: Notification[];
  unreadCount: number;
  page: number;
  pageSize: number;
}

/**
 * Get notifications
 */
export const getNotifications = (page = 1, pageSize = 20, isRead?: boolean): Promise<NotificationListResponse> => {
  return get<NotificationListResponse>('/notifications', { page, pageSize, isRead });
};

/**
 * Get unread notification count
 */
export const getUnreadCount = (): Promise<{ count: number }> => {
  return get<{ count: number }>('/notifications/unread-count');
};

/**
 * Mark notification as read
 */
export const markAsRead = (id: string): Promise<{ message: string }> => {
  return put<{ message: string }>(`/notifications/${id}/read`, {});
};

/**
 * Mark all notifications as read
 */
export const markAllAsRead = (): Promise<{ message: string }> => {
  return put<{ message: string }>('/notifications/read-all', {});
};
