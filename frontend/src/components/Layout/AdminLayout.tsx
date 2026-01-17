import React, { useState, useEffect } from 'react';
import { Layout, Menu, Button, Drawer } from 'antd';
import {
  DashboardOutlined,
  ShoppingOutlined,
  TeamOutlined,
  OrderedListOutlined,
  BarChartOutlined,
  AuditOutlined,
  WalletOutlined,
  MenuOutlined,
} from '@ant-design/icons';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import type { MenuProps } from 'antd';
import './AdminLayout.css';

const { Header, Sider, Content } = Layout;

const AdminLayout: React.FC = () => {
  const [collapsed, setCollapsed] = useState(false);
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const handleResize = () => {
      const mobile = window.innerWidth < 768;
      setIsMobile(mobile);
      if (!mobile) {
        setDrawerVisible(false);
      }
    };
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const menuItems: MenuProps['items'] = [
    { key: '/admin', icon: <DashboardOutlined />, label: '仪表盘' },
    { key: '/admin/products', icon: <ShoppingOutlined />, label: '产品管理' },
    { key: '/admin/employees', icon: <TeamOutlined />, label: '员工管理' },
    { key: '/admin/points', icon: <WalletOutlined />, label: '积分管理' },
    { key: '/admin/orders', icon: <OrderedListOutlined />, label: '订单管理' },
    { key: '/admin/statistics', icon: <BarChartOutlined />, label: '数据统计' },
    { key: '/admin/audit-logs', icon: <AuditOutlined />, label: '操作日志' },
  ];

  const handleMenuClick: MenuProps['onClick'] = (e) => {
    navigate(e.key);
    if (isMobile) {
      setDrawerVisible(false);
    }
  };

  return (
    <Layout className="admin-layout">
      {!isMobile ? (
        <Sider 
          collapsible 
          collapsed={collapsed} 
          onCollapse={setCollapsed}
          className="admin-sider"
        >
          <div className="admin-logo">
            {collapsed ? 'AWS' : 'AWSomeShop'}
          </div>
          <Menu
            theme="dark"
            mode="inline"
            selectedKeys={[location.pathname]}
            items={menuItems}
            onClick={handleMenuClick}
          />
        </Sider>
      ) : (
        <Drawer
          title="管理菜单"
          placement="left"
          onClose={() => setDrawerVisible(false)}
          open={drawerVisible}
          bodyStyle={{ padding: 0 }}
        >
          <Menu
            mode="inline"
            selectedKeys={[location.pathname]}
            items={menuItems}
            onClick={handleMenuClick}
          />
        </Drawer>
      )}
      <Layout>
        <Header className="admin-header">
          {isMobile && (
            <Button
              type="text"
              icon={<MenuOutlined />}
              onClick={() => setDrawerVisible(true)}
              className="mobile-menu-button"
            />
          )}
          <span className="admin-title">管理员</span>
        </Header>
        <Content className="admin-content">
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
};

export default AdminLayout;
