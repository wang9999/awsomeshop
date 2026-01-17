import React, { useEffect, useState } from 'react';
import { Layout, Menu, Badge, Avatar, Dropdown, Space, Drawer, Button } from 'antd';
import {
  ShoppingCartOutlined,
  BellOutlined,
  UserOutlined,
  HomeOutlined,
  AppstoreOutlined,
  HistoryOutlined,
  WalletOutlined,
  MenuOutlined,
} from '@ant-design/icons';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import type { MenuProps } from 'antd';
import PointsBalance from '../PointsBalance/PointsBalance';
import { useAuthStore } from '../../store/authSlice';
import { useCartStore } from '../../store/cartSlice';
import './MainLayout.css';

const { Header, Content, Footer } = Layout;

const MainLayout: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { logout } = useAuthStore();
  const { items, fetchCart } = useCartStore();
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);

  useEffect(() => {
    fetchCart();
  }, [fetchCart]);

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 768);
    };
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const menuItems: MenuProps['items'] = [
    { key: '/', icon: <HomeOutlined />, label: '首页' },
    { key: '/products', icon: <AppstoreOutlined />, label: '产品列表' },
    { key: '/orders', icon: <HistoryOutlined />, label: '兑换历史' },
    { key: '/points', icon: <WalletOutlined />, label: '积分明细' },
  ];

  const handleUserMenuClick: MenuProps['onClick'] = (e) => {
    if (e.key === 'logout') {
      logout();
      navigate('/login');
    } else if (e.key === 'profile') {
      navigate('/profile');
    } else if (e.key === 'addresses') {
      navigate('/addresses');
    }
  };

  const userMenuItems: MenuProps['items'] = [
    { key: 'profile', label: '个人信息' },
    { key: 'addresses', label: '收货地址' },
    { key: 'logout', label: '退出登录' },
  ];

  const handleMenuClick: MenuProps['onClick'] = (e) => {
    navigate(e.key);
    setDrawerVisible(false);
  };

  return (
    <Layout className="main-layout">
      <Header className="main-header">
        <div className="header-content">
          {isMobile && (
            <Button
              type="text"
              icon={<MenuOutlined />}
              onClick={() => setDrawerVisible(true)}
              className="mobile-menu-button"
            />
          )}
          <div className="logo">AWSomeShop</div>
          {!isMobile && (
            <Menu
              theme="dark"
              mode="horizontal"
              selectedKeys={[location.pathname]}
              items={menuItems}
              onClick={handleMenuClick}
              className="desktop-menu"
            />
          )}
          <Space size={isMobile ? 'middle' : 'large'} className="header-actions">
            <Badge count={items.length} size="small">
              <ShoppingCartOutlined
                className="header-icon"
                onClick={() => navigate('/cart')}
              />
            </Badge>
            <Badge count={0} size="small">
              <BellOutlined className="header-icon" />
            </Badge>
            <Dropdown menu={{ items: userMenuItems, onClick: handleUserMenuClick }} placement="bottomRight">
              <Avatar icon={<UserOutlined />} className="user-avatar" />
            </Dropdown>
          </Space>
        </div>
      </Header>

      <Drawer
        title="菜单"
        placement="left"
        onClose={() => setDrawerVisible(false)}
        open={drawerVisible}
        className="mobile-drawer"
      >
        <Menu
          mode="vertical"
          selectedKeys={[location.pathname]}
          items={menuItems}
          onClick={handleMenuClick}
        />
      </Drawer>

      <Content className="main-content">
        <div className="points-balance-wrapper">
          <PointsBalance />
        </div>
        <Outlet />
      </Content>
      <Footer className="main-footer">
        AWSomeShop ©{new Date().getFullYear()} - 员工福利平台
      </Footer>
    </Layout>
  );
};

export default MainLayout;
