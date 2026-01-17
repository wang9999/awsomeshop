import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button, Card, Input, message, Radio, Space, Spin, Typography } from 'antd';
import { useCartStore } from '../../../store/cartSlice';
import { useAddressStore } from '../../../store/addressSlice';
import { getAddresses } from '../../../services/addressService';
import { createOrder } from '../../../services/orderService';
import './Checkout.css';

const { Title, Text } = Typography;

export default function Checkout() {
  const navigate = useNavigate();
  const { items, totalPoints, clearCart } = useCartStore();
  const { addresses, setAddresses, setLoading: setAddressLoading } = useAddressStore();
  
  const [selectedAddressId, setSelectedAddressId] = useState<string>('');
  const [captcha, setCaptcha] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadAddresses();
  }, []);

  useEffect(() => {
    // Auto-select default address
    const defaultAddr = addresses.find(addr => addr.isDefault);
    if (defaultAddr) {
      setSelectedAddressId(defaultAddr.id);
    } else if (addresses.length > 0) {
      setSelectedAddressId(addresses[0].id);
    }
  }, [addresses]);

  const loadAddresses = async () => {
    try {
      setAddressLoading(true);
      const data = await getAddresses();
      setAddresses(data);
    } catch (error) {
      message.error('加载地址失败');
    } finally {
      setAddressLoading(false);
    }
  };

  const handleSubmit = async () => {
    if (!selectedAddressId) {
      message.warning('请选择收货地址');
      return;
    }

    if (!captcha) {
      message.warning('请输入验证码');
      return;
    }

    if (items.length === 0) {
      message.warning('购物车为空');
      return;
    }

    try {
      setLoading(true);
      const orderData = {
        items: items.map(item => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
        addressId: selectedAddressId,
        captcha,
      };

      const order = await createOrder(orderData);
      message.success('兑换成功！');
      clearCart();
      navigate(`/employee/orders/${order.id}`);
    } catch (error: any) {
      message.error(error.response?.data?.message || '兑换失败');
    } finally {
      setLoading(false);
    }
  };

  if (items.length === 0) {
    return (
      <div className="checkout-empty">
        <Text>购物车为空</Text>
        <Button type="primary" onClick={() => navigate('/employee/products')}>
          去购物
        </Button>
      </div>
    );
  }

  return (
    <div className="checkout-container">
      <Title level={2}>确认兑换</Title>

      <Card title="商品信息" className="checkout-section">
        {items.map(item => (
          <div key={item.productId} className="checkout-item">
            <img src={item.productImage || '/placeholder.png'} alt={item.productName} />
            <div className="item-info">
              <Text strong>{item.productName}</Text>
              <Text type="secondary">数量: {item.quantity}</Text>
            </div>
            <Text strong>{item.points * item.quantity} 积分</Text>
          </div>
        ))}
        <div className="checkout-total">
          <Text strong>总计: {totalPoints} 积分</Text>
        </div>
      </Card>

      <Card title="收货地址" className="checkout-section">
        {addresses.length === 0 ? (
          <div>
            <Text>暂无收货地址</Text>
            <Button type="link" onClick={() => navigate('/employee/addresses')}>
              添加地址
            </Button>
          </div>
        ) : (
          <Radio.Group value={selectedAddressId} onChange={(e) => setSelectedAddressId(e.target.value)}>
            <Space direction="vertical" style={{ width: '100%' }}>
              {addresses.map(addr => (
                <Radio key={addr.id} value={addr.id}>
                  <div className="address-item">
                    <Text strong>{addr.receiverName} {addr.receiverPhone}</Text>
                    <Text>
                      {addr.province} {addr.city} {addr.district} {addr.detailAddress}
                    </Text>
                    {addr.isDefault && <Text type="success">[默认]</Text>}
                  </div>
                </Radio>
              ))}
            </Space>
          </Radio.Group>
        )}
      </Card>

      <Card title="验证码" className="checkout-section">
        <Input
          placeholder="请输入验证码"
          value={captcha}
          onChange={(e) => setCaptcha(e.target.value)}
          style={{ width: 200 }}
        />
        <Text type="secondary" style={{ marginLeft: 16 }}>
          （简化实现，输入任意内容即可）
        </Text>
      </Card>

      <div className="checkout-actions">
        <Button onClick={() => navigate('/employee/cart')}>返回购物车</Button>
        <Button type="primary" size="large" loading={loading} onClick={handleSubmit}>
          确认兑换
        </Button>
      </div>
    </div>
  );
}
