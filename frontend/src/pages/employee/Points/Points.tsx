import React, { useState, useEffect } from 'react';
import { Card, Table, DatePicker, Button, Space, Tag, message } from 'antd';
import { DownloadOutlined, ReloadOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import dayjs, { Dayjs } from 'dayjs';
import { getPointsTransactions, exportPointsTransactions, type PointsTransaction } from '../../../services/pointsService';
import PointsBalance from '../../../components/PointsBalance/PointsBalance';

const { RangePicker } = DatePicker;

const Points: React.FC = () => {
  const [transactions, setTransactions] = useState<PointsTransaction[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 20,
    total: 0,
  });
  const [dateRange, setDateRange] = useState<[Dayjs | null, Dayjs | null]>([null, null]);

  const fetchTransactions = async (page = 1, pageSize = 20) => {
    setLoading(true);
    try {
      const params: any = {
        pageNumber: page,
        pageSize,
      };

      if (dateRange[0] && dateRange[1]) {
        params.startDate = dateRange[0].format('YYYY-MM-DD');
        params.endDate = dateRange[1].format('YYYY-MM-DD');
      }

      const data = await getPointsTransactions(params);
      setTransactions(data.transactions);
      setPagination({
        current: data.pageNumber,
        pageSize: data.pageSize,
        total: data.totalCount,
      });
    } catch (error) {
      message.error('加载积分历史失败');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, []);

  const handleTableChange = (newPagination: any) => {
    fetchTransactions(newPagination.current, newPagination.pageSize);
  };

  const handleDateRangeChange = (dates: any) => {
    setDateRange(dates || [null, null]);
  };

  const handleSearch = () => {
    fetchTransactions(1, pagination.pageSize);
  };

  const handleReset = () => {
    setDateRange([null, null]);
    fetchTransactions(1, pagination.pageSize);
  };

  const handleExport = async () => {
    try {
      const params: any = {};
      if (dateRange[0] && dateRange[1]) {
        params.startDate = dateRange[0].format('YYYY-MM-DD');
        params.endDate = dateRange[1].format('YYYY-MM-DD');
      }

      const blob = await exportPointsTransactions(params);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `points_transactions_${dayjs().format('YYYYMMDD_HHmmss')}.csv`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
      message.success('导出成功');
    } catch (error) {
      message.error('导出失败');
    }
  };

  const getTypeColor = (type: string) => {
    switch (type) {
      case '发放':
        return 'green';
      case '扣除':
        return 'red';
      case '消费':
        return 'orange';
      case '退回':
        return 'blue';
      case '过期':
        return 'default';
      default:
        return 'default';
    }
  };

  const columns: ColumnsType<PointsTransaction> = [
    {
      title: '交易时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 180,
      render: (text: string) => dayjs(text).format('YYYY-MM-DD HH:mm:ss'),
    },
    {
      title: '类型',
      dataIndex: 'type',
      key: 'type',
      width: 100,
      render: (text: string) => <Tag color={getTypeColor(text)}>{text}</Tag>,
    },
    {
      title: '积分变动',
      dataIndex: 'amount',
      key: 'amount',
      width: 120,
      render: (amount: number) => (
        <span style={{ color: amount > 0 ? '#52c41a' : '#ff4d4f', fontWeight: 'bold' }}>
          {amount > 0 ? '+' : ''}{amount}
        </span>
      ),
    },
    {
      title: '原因',
      dataIndex: 'reason',
      key: 'reason',
      ellipsis: true,
    },
    {
      title: '操作者',
      dataIndex: 'operatorType',
      key: 'operatorType',
      width: 100,
      render: (text: string) => text === 'System' ? '系统' : '管理员',
    },
    {
      title: '余额变化',
      key: 'balance',
      width: 150,
      render: (_, record) => `${record.balanceBefore} → ${record.balanceAfter}`,
    },
    {
      title: '过期时间',
      dataIndex: 'expiryDate',
      key: 'expiryDate',
      width: 120,
      render: (text: string) => text ? dayjs(text).format('YYYY-MM-DD') : '-',
    },
  ];

  return (
    <div>
      <PointsBalance />
      
      <Card 
        title="积分明细" 
        style={{ marginTop: '24px' }}
        extra={
          <Space>
            <RangePicker
              value={dateRange}
              onChange={handleDateRangeChange}
              format="YYYY-MM-DD"
            />
            <Button onClick={handleSearch}>查询</Button>
            <Button onClick={handleReset}>重置</Button>
            <Button 
              type="primary" 
              icon={<DownloadOutlined />}
              onClick={handleExport}
            >
              导出
            </Button>
            <Button 
              icon={<ReloadOutlined />}
              onClick={() => fetchTransactions(pagination.current, pagination.pageSize)}
            >
              刷新
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={transactions}
          rowKey="id"
          loading={loading}
          pagination={pagination}
          onChange={handleTableChange}
          scroll={{ x: 1000 }}
        />
      </Card>
    </div>
  );
};

export default Points;
