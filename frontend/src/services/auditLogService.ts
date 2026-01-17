import { get } from '../utils/request';

export interface AuditLog {
  id: string;
  adminId: string;
  adminName: string;
  action: string;
  entityType: string;
  entityId?: string;
  oldValue?: string;
  newValue?: string;
  ipAddress?: string;
  userAgent?: string;
  createdAt: string;
}

export interface AuditLogListResponse {
  logs: AuditLog[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AuditLogQueryParams {
  page?: number;
  pageSize?: number;
  adminId?: string;
  action?: string;
  entityType?: string;
  startDate?: string;
  endDate?: string;
}

/**
 * Admin: Get audit logs
 */
export const getAuditLogs = (params?: AuditLogQueryParams): Promise<AuditLogListResponse> => {
  return get<AuditLogListResponse>('/admin/audit-logs', params);
};

/**
 * Admin: Get audit logs for specific entity
 */
export const getAuditLogsByEntity = (entityType: string, entityId: string): Promise<{ logs: AuditLog[] }> => {
  return get<{ logs: AuditLog[] }>(`/admin/audit-logs/entity/${entityType}/${entityId}`);
};
