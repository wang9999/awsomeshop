import React from 'react';
import { Form, Input, Button, Card, Typography, message } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../../store/authSlice';

const { Title, Text } = Typography;

interface LoginForm {
  username: string;
  password: string;
}

const AdminLogin: React.FC = () => {
  const navigate = useNavigate();
  const { adminLogin, loading } = useAuthStore();

  const onFinish = async (values: LoginForm) => {
    try {
      await adminLogin(values.username, values.password);
      message.success('登录成功');
      navigate('/admin');
    } catch {
      message.error('登录失败，请检查账号和密码');
    }
  };

  return (
    <div style={{
      minHeight: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 100%)',
    }}>
      <Card style={{ width: 400, boxShadow: '0 4px 12px rgba(0,0,0,0.15)' }}>
        <div style={{ textAlign: 'center', marginBottom: '24px' }}>
          <Title level={2} style={{ marginBottom: '8px' }}>AWSomeShop</Title>
          <Text type="secondary">管理后台</Text>
        </div>
        <Form
          name="admin-login"
          onFinish={onFinish}
          autoComplete="off"
          layout="vertical"
        >
          <Form.Item
            name="username"
            rules={[{ required: true, message: '请输入管理员账号' }]}
          >
            <Input
              prefix={<UserOutlined />}
              placeholder="管理员账号"
              size="large"
            />
          </Form.Item>
          <Form.Item
            name="password"
            rules={[{ required: true, message: '请输入密码' }]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="密码"
              size="large"
            />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block size="large">
              登录
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default AdminLogin;
