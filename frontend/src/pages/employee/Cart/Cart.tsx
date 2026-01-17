import React, { useEffect } from 'react';
import { Card, Table, InputNumber, Button, Typography, Empty, Space, message } from 'antd';
import { DeleteOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useCartStore } from '../../../store/cartSlice';
import type { CartItem } from '../../../services/cartService';
import type { ColumnsType } from 'antd/es/table';

const { Title, Text } = Typography;

const Cart: React.FC = () => {
  const navigate = useNavigate();
  const { items, totalPoints, loading, fetchCart, updateQuantity, removeItem } = useCartStore();

  useEffect(() => {
    fetchCart();
  }, [fetchCart]);

  const handleQuantityChange = async (itemId: string, quantity: number) => {
    try {
      await updateQuantity(itemId, quantity);
    } catch {
      message.error('更新数量失败');
    }
  };

  const handleRemove = async (itemId: string) => {
    try {
      await removeItem(itemId);
      message.success('已从购物车移除');
    } catch {
      message.error('移除失败');
    }
  };

  const columns: ColumnsType<CartItem> = [
    {
      title: '产品',
      dataIndex: 'productName',
      key: 'productName',
      render: (name: string, record: CartItem) => (
        <Space>
          <img
            src={record.productImage}
            alt={name}
            style={{ width: '60px', height: '60px', objectFit: 'cover' }}
          />
          <span>{name}</span>
        </Space>
      ),
    },
    {
      title: '积分',
      dataIndex: 'points',
      key: 'points',
      render: (points: number) => <Text strong style={{ color: '#ff6600' }}>{points}</Text>,
    },
    {
      title: '数量',
      dataIndex: 'quantity',
      key: 'quantity',
      render: (quantity: number, record: CartItem) => (
        <InputNumber
          min={1}
          max={record.stock}
          value={quantity}
          onChange={(value) => handleQuantityChange(record.id, value || 1)}
          disabled={record.stock === 0}
        />
      ),
    },
    {
      title: '小计',
      key: 'subtotal',
      render: (_: unknown, record: CartItem) => (
        <Text strong style={{ color: '#ff6600' }}>{record.points * record.quantity}</Text>
      ),
    },
    {
      title: '操作',
      key: 'action',
      render: (_: unknown, record: CartItem) => (
        <Button
          type="text"
          danger
          icon={<DeleteOutlined />}
          onClick={() => handleRemove(record.id)}
        >
          删除
        </Button>
      ),
    },
  ];

  if (items.length === 0) {
    return (
      <Card>
        <Empty description="购物车是空的">
          <Button type="primary" onClick={() => navigate('/employee/products')}>
            去选购
          </Button>
        </Empty>
      </Card>
    );
  }

  return (
    <div>
      <Title level={3}>购物车</Title>
      <Card>
        <Table
          columns={columns}
          dataSource={items}
          rowKey="id"
          loading={loading}
          pagination={false}
        />
        <div style={{ marginTop: '24px', textAlign: 'right' }}>
          <Space size="large">
            <Text>
              共 <Text strong>{items.length}</Text> 件商品
            </Text>
            <Text>
              总计: <Text strong style={{ fontSize: '24px', color: '#ff6600' }}>{totalPoints}</Text> 积分
            </Text>
            <Button type="primary" size="large" onClick={() => navigate('/employee/checkout')}>
              去结算
            </Button>
          </Space>
        </div>
      </Card>
    </div>
  );
};

export default Cart;
