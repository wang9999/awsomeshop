import { useState, useEffect } from 'react';
import { Card, Table, Input, Select, DatePicker, Button, Space, Tag } from 'antd';
import { getAuditLogs, AuditLog } from '../../../services/auditLogService';
import dayjs from 'dayjs';

const { RangePicker } = DatePicker;

export default function AuditLogs() {
  const [logs, setLogs] = useState<AuditLog[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({ current: 1, pageSize: 20, total: 0 });
  const [filters, setFilters] = useState({
    action: '',
    entityType: '',
    startDate: '',
    endDate: '',
  });

  useEffect(() => {
    loadLogs();
  }, [pagination.current, filters]);

  const loadLogs = async () => {
    try {
      setLoading(true);
      const data = await getAuditLogs({
        page: pagination.current,
        pageSize: pagination.pageSize,
        ...filters,
      });
      setLogs(data.logs);
      setPagination(prev => ({ ...prev, total: data.totalCount }));
    } catch (error) {
      console.error('Failed to load audit logs', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDateChange = (dates: any) => {
    if (dates) {
      setFilters(prev => ({
        ...prev,
        startDate: dates[0].toISOString(),
        endDate: dates[1].toISOString(),
      }));
    } else {
      setFilters(prev => ({ ...prev, startDate: '', endDate: '' }));
    }
  };

  const getActionColor = (action: string) => {
    if (action.includes('创建') || action.includes('Create')) return 'green';
    if (action.includes('更新') || action.includes('Update')) return 'blue';
    if (action.includes('删除') || action.includes('Delete')) return 'red';
    return 'default';
  };

  const columns = [
    {
      title: '时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 180,
      render: (date: string) => dayjs(date).format('YYYY-MM-DD HH:mm:ss'),
    },
    {
      title: '操作人',
      dataIndex: 'adminName',
      key: 'adminName',
      width: 120,
    },
    {
      title: '操作',
      dataIndex: 'action',
      key: 'action',
      width: 150,
      render: (action: string) => <Tag color={getActionColor(action)}>{action}</Tag>,
    },
    {
      title: '实体类型',
      dataIndex: 'entityType',
      key: 'entityType',
      width: 120,
    },
    {
      title: '实体ID',
      dataIndex: 'entityId',
      key: 'entityId',
      width: 200,
      ellipsis: true,
    },
    {
      title: 'IP地址',
      dataIndex: 'ipAddress',
      key: 'ipAddress',
      width: 140,
    },
    {
      title: '详情',
      key: 'details',
      render: (_: any, record: AuditLog) => (
        <Space direction="vertical" size="small">
          {record.oldValue && (
            <div>
              <span style={{ color: '#999' }}>旧值: </span>
              <span style={{ fontSize: 12 }}>{record.oldValue.substring(0, 50)}...</span>
            </div>
          )}
          {record.newValue && (
            <div>
              <span style={{ color: '#999' }}>新值: </span>
              <span style={{ fontSize: 12 }}>{record.newValue.substring(0, 50)}...</span>
            </div>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <Card
        title="操作日志"
        extra={
          <Space>
            <Input
              placeholder="搜索操作"
              style={{ width: 150 }}
              value={filters.action}
              onChange={(e) => setFilters(prev => ({ ...prev, action: e.target.value }))}
              allowClear
            />
            <Select
              placeholder="实体类型"
              style={{ width: 120 }}
              value={filters.entityType || undefined}
              onChange={(value) => setFilters(prev => ({ ...prev, entityType: value || '' }))}
              allowClear
            >
              <Select.Option value="Product">产品</Select.Option>
              <Select.Option value="Order">订单</Select.Option>
              <Select.Option value="Employee">员工</Select.Option>
              <Select.Option value="Points">积分</Select.Option>
            </Select>
            <RangePicker
              onChange={handleDateChange}
              format="YYYY-MM-DD"
            />
            <Button onClick={loadLogs}>刷新</Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={logs}
          rowKey="id"
          loading={loading}
          pagination={{
            ...pagination,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条记录`,
            onChange: (page, pageSize) => setPagination(prev => ({ ...prev, current: page, pageSize })),
          }}
          scroll={{ x: 1200 }}
        />
      </Card>
    </div>
  );
}
