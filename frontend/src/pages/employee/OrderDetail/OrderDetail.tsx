import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button, Card, Descriptions, Tag, message, Modal, Input } from 'antd';
import { getOrderById, cancelOrder, Order } from '../../../services/orderService';
import dayjs from 'dayjs';

export default function OrderDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(false);
  const [cancelModalVisible, setCancelModalVisible] = useState(false);
  const [cancelReason, setCancelReason] = useState('');

  useEffect(() => {
    if (id) {
      loadOrder();
    }
  }, [id]);

  const loadOrder = async () => {
    if (!id) return;
    try {
      setLoading(true);
      const data = await getOrderById(id);
      setOrder(data);
    } catch (error) {
      message.error('加载订单失败');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async () => {
    if (!id) return;
    try {
      setLoading(true);
      await cancelOrder(id, cancelReason);
      message.success('订单已取消');
      setCancelModalVisible(false);
      loadOrder();
    } catch (error: any) {
      message.error(error.response?.data?.message || '取消订单失败');
    } finally {
      setLoading(false);
    }
  };

  const canCancel = order?.status === '待发货' && 
    dayjs().diff(dayjs(order.createdAt), 'hour') < 24;

  if (!order) {
    return <div style={{ padding: 24 }}>加载中...</div>;
  }

  return (
    <div style={{ padding: 24, maxWidth: 800, margin: '0 auto' }}>
      <Card
        title={`订单详情 - ${order.orderNumber}`}
        extra={
          canCancel && (
            <Button danger onClick={() => setCancelModalVisible(true)}>
              取消订单
            </Button>
          )
        }
      >
        <Descriptions column={1} bordered>
          <Descriptions.Item label="订单状态">
            <Tag color={order.status === '已完成' ? 'green' : 'blue'}>{order.status}</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="总积分">{order.totalPoints}</Descriptions.Item>
          <Descriptions.Item label="创建时间">
            {dayjs(order.createdAt).format('YYYY-MM-DD HH:mm:ss')}
          </Descriptions.Item>
          {order.trackingNumber && (
            <Descriptions.Item label="物流单号">{order.trackingNumber}</Descriptions.Item>
          )}
          {order.address && (
            <Descriptions.Item label="收货地址">
              {order.address.receiverName} {order.address.receiverPhone}<br />
              {order.address.province} {order.address.city} {order.address.district} {order.address.detailAddress}
            </Descriptions.Item>
          )}
        </Descriptions>

        <Card title="商品列表" style={{ marginTop: 16 }}>
          {order.items.map(item => (
            <div key={item.productId} style={{ padding: '12px 0', borderBottom: '1px solid #f0f0f0' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <span>{item.productName} x {item.quantity}</span>
                <span>{item.points * item.quantity} 积分</span>
              </div>
            </div>
          ))}
        </Card>

        <div style={{ marginTop: 24, textAlign: 'center' }}>
          <Button onClick={() => navigate('/employee/orders')}>返回订单列表</Button>
        </div>
      </Card>

      <Modal
        title="取消订单"
        open={cancelModalVisible}
        onOk={handleCancel}
        onCancel={() => setCancelModalVisible(false)}
        confirmLoading={loading}
      >
        <Input.TextArea
          placeholder="请输入取消原因（可选）"
          value={cancelReason}
          onChange={(e) => setCancelReason(e.target.value)}
          rows={4}
        />
      </Modal>
    </div>
  );
}
