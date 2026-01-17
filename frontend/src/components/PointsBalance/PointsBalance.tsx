import React, { useEffect } from 'react';
import { Card, Statistic, Spin, Alert, Tooltip } from 'antd';
import { WalletOutlined, ClockCircleOutlined } from '@ant-design/icons';
import { usePointsStore } from '../../store/pointsSlice';
import './PointsBalance.css';

interface PointsBalanceProps {
  showExpiring?: boolean;
  size?: 'small' | 'default' | 'large';
}

const PointsBalance: React.FC<PointsBalanceProps> = ({ 
  showExpiring = true, 
  size = 'default' 
}) => {
  const { balance, expiringPoints, loading, error, fetchBalance } = usePointsStore();

  useEffect(() => {
    fetchBalance();
  }, [fetchBalance]);

  if (loading && balance === 0) {
    return (
      <Card className="points-balance-card">
        <Spin />
      </Card>
    );
  }

  if (error) {
    return (
      <Card className="points-balance-card">
        <Alert message="加载失败" description={error} type="error" showIcon />
      </Card>
    );
  }

  return (
    <div className={`points-balance-container ${size}`}>
      <Card className="points-balance-card" bordered={false}>
        <Statistic
          title="我的积分"
          value={balance}
          prefix={<WalletOutlined />}
          valueStyle={{ color: '#ff9900' }}
        />
      </Card>
      
      {showExpiring && expiringPoints > 0 && (
        <Card className="points-expiring-card" bordered={false}>
          <Tooltip title="30天内即将过期的积分">
            <Statistic
              title="即将过期"
              value={expiringPoints}
              prefix={<ClockCircleOutlined />}
              valueStyle={{ color: '#ff4d4f', fontSize: '16px' }}
            />
          </Tooltip>
        </Card>
      )}
    </div>
  );
};

export default PointsBalance;
