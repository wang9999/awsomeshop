import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button, Card, Table, Tag, message, Select, DatePicker } from 'antd';
import { getMyOrders, Order } from '../../../services/orderService';
import dayjs from 'dayjs';

const { RangePicker } = DatePicker;

export default function Orders() {
  const navigate = useNavigate();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({ current: 1, pageSize: 10, total: 0 });
  const [status, setStatus] = useState<string>('');

  useEffect(() => {
    loadOrders();
  }, [pagination.current, status]);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = await getMyOrders({
        page: pagination.current,
        pageSize: pagination.pageSize,
        status: status || undefined,
      });
      setOrders(data.orders);
      setPagination(prev => ({ ...prev, total: data.totalCount }));
    } catch (error) {
      message.error('加载订单失败');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    const colors: Record<string, string> = {
      '待确认': 'blue',
      '待发货': 'orange',
      '已发货': 'cyan',
      '已完成': 'green',
      '已取消': 'red',
    };
    return colors[status] || 'default';
  };

  const columns = [
    {
      title: '订单号',
      dataIndex: 'orderNumber',
      key: 'orderNumber',
    },
    {
      title: '商品',
      dataIndex: 'items',
      key: 'items',
      render: (items: any[]) => items.map(item => item.productName).join(', '),
    },
    {
      title: '积分',
      dataIndex: 'totalPoints',
      key: 'totalPoints',
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => <Tag color={getStatusColor(status)}>{status}</Tag>,
    },
    {
      title: '创建时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (date: string) => dayjs(date).format('YYYY-MM-DD HH:mm'),
    },
    {
      title: '操作',
      key: 'action',
      render: (_: any, record: Order) => (
        <Button type="link" onClick={() => navigate(`/employee/orders/${record.id}`)}>
          查看详情
        </Button>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <Card
        title="我的订单"
        extra={
          <Select
            style={{ width: 120 }}
            placeholder="订单状态"
            allowClear
            value={status || undefined}
            onChange={setStatus}
          >
            <Select.Option value="待确认">待确认</Select.Option>
            <Select.Option value="待发货">待发货</Select.Option>
            <Select.Option value="已发货">已发货</Select.Option>
            <Select.Option value="已完成">已完成</Select.Option>
            <Select.Option value="已取消">已取消</Select.Option>
          </Select>
        }
      >
        <Table
          columns={columns}
          dataSource={orders}
          rowKey="id"
          loading={loading}
          pagination={{
            ...pagination,
            onChange: (page) => setPagination(prev => ({ ...prev, current: page })),
          }}
        />
      </Card>
    </div>
  );
}
