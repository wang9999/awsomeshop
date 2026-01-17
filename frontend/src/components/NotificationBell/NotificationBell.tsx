import { useState, useEffect } from 'react';
import { Badge, Dropdown, List, Button, Typography, Empty } from 'antd';
import { BellOutlined } from '@ant-design/icons';
import { getNotifications, markAsRead, markAllAsRead, Notification } from '../../services/notificationService';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import 'dayjs/locale/zh-cn';

dayjs.extend(relativeTime);
dayjs.locale('zh-cn');

const { Text } = Typography;

export default function NotificationBell() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    loadNotifications();
    // 每30秒刷新一次
    const interval = setInterval(loadNotifications, 30000);
    return () => clearInterval(interval);
  }, []);

  const loadNotifications = async () => {
    try {
      const data = await getNotifications(1, 10);
      setNotifications(data.notifications);
      setUnreadCount(data.unreadCount);
    } catch (error) {
      console.error('Failed to load notifications', error);
    }
  };

  const handleMarkAsRead = async (id: string) => {
    try {
      await markAsRead(id);
      loadNotifications();
    } catch (error) {
      console.error('Failed to mark as read', error);
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      setLoading(true);
      await markAllAsRead();
      loadNotifications();
    } catch (error) {
      console.error('Failed to mark all as read', error);
    } finally {
      setLoading(false);
    }
  };

  const getTypeColor = (type: string) => {
    const colors: Record<string, string> = {
      '积分变动': '#52c41a',
      '订单状态': '#1890ff',
      '产品上新': '#fa8c16',
      '系统通知': '#722ed1',
    };
    return colors[type] || '#666';
  };

  const menu = (
    <div style={{ width: 360, maxHeight: 480, overflow: 'auto', backgroundColor: 'white', borderRadius: 8, boxShadow: '0 2px 8px rgba(0,0,0,0.15)' }}>
      <div style={{ padding: '12px 16px', borderBottom: '1px solid #f0f0f0', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Text strong>通知</Text>
        {unreadCount > 0 && (
          <Button type="link" size="small" onClick={handleMarkAllAsRead} loading={loading}>
            全部已读
          </Button>
        )}
      </div>
      {notifications.length === 0 ? (
        <Empty description="暂无通知" style={{ padding: 24 }} />
      ) : (
        <List
          dataSource={notifications}
          renderItem={(item) => (
            <List.Item
              style={{
                padding: '12px 16px',
                cursor: 'pointer',
                backgroundColor: item.isRead ? 'white' : '#f0f5ff',
              }}
              onClick={() => !item.isRead && handleMarkAsRead(item.id)}
            >
              <div style={{ width: '100%' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                  <Text strong style={{ fontSize: 14 }}>{item.title}</Text>
                  {!item.isRead && <Badge status="processing" />}
                </div>
                <Text type="secondary" style={{ fontSize: 12, display: 'block', marginBottom: 4 }}>
                  {item.content}
                </Text>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Text type="secondary" style={{ fontSize: 12 }}>
                    {dayjs(item.createdAt).fromNow()}
                  </Text>
                  <Text style={{ fontSize: 12, color: getTypeColor(item.type) }}>
                    {item.type}
                  </Text>
                </div>
              </div>
            </List.Item>
          )}
        />
      )}
    </div>
  );

  return (
    <Dropdown
      dropdownRender={() => menu}
      trigger={['click']}
      open={open}
      onOpenChange={setOpen}
    >
      <Badge count={unreadCount} offset={[-5, 5]}>
        <Button
          type="text"
          icon={<BellOutlined style={{ fontSize: 20 }} />}
          style={{ border: 'none' }}
        />
      </Badge>
    </Dropdown>
  );
}
