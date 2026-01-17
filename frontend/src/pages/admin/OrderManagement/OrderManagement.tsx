import { useState, useEffect } from 'react';
import { Button, Card, Table, Tag, message, Select, Modal, Input, Form } from 'antd';
import { getAllOrders, updateOrderStatus, Order } from '../../../services/orderService';
import dayjs from 'dayjs';

export default function OrderManagement() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({ current: 1, pageSize: 10, total: 0 });
  const [status, setStatus] = useState<string>('');
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    loadOrders();
  }, [pagination.current, status]);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = await getAllOrders({
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

  const handleUpdateStatus = async (values: { status: string; trackingNumber?: string }) => {
    if (!selectedOrder) return;
    try {
      await updateOrderStatus(selectedOrder.id, values.status, values.trackingNumber);
      message.success('订单状态更新成功');
      setModalVisible(false);
      form.resetFields();
      loadOrders();
    } catch (error: any) {
      message.error(error.response?.data?.message || '更新失败');
    }
  };

  const openStatusModal = (order: Order) => {
    setSelectedOrder(order);
    form.setFieldsValue({ status: order.status, trackingNumber: order.trackingNumber });
    setModalVisible(true);
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
        <Button type="link" onClick={() => openStatusModal(record)}>
          更新状态
        </Button>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <Card
        title="订单管理"
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

      <Modal
        title="更新订单状态"
        open={modalVisible}
        onCancel={() => {
          setModalVisible(false);
          form.resetFields();
        }}
        onOk={() => form.submit()}
      >
        <Form form={form} layout="vertical" onFinish={handleUpdateStatus}>
          <Form.Item name="status" label="订单状态" rules={[{ required: true }]}>
            <Select>
              <Select.Option value="待确认">待确认</Select.Option>
              <Select.Option value="待发货">待发货</Select.Option>
              <Select.Option value="已发货">已发货</Select.Option>
              <Select.Option value="已完成">已完成</Select.Option>
              <Select.Option value="已取消">已取消</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item name="trackingNumber" label="物流单号">
            <Input placeholder="请输入物流单号（可选）" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
