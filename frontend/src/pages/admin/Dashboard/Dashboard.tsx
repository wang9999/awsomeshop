import { useState, useEffect } from 'react';
import { Card, Row, Col, Statistic, DatePicker, Space, Table, Spin } from 'antd';
import {
  ShoppingOutlined,
  UserOutlined,
  GiftOutlined,
  TrophyOutlined,
} from '@ant-design/icons';
import {
  getOverviewStatistics,
  getTopProducts,
  getActiveEmployees,
  OverviewStatistics,
  ProductStatistics,
  EmployeeStatistics,
} from '../../../services/statisticsService';
import dayjs from 'dayjs';
import './Dashboard.css';

const { RangePicker } = DatePicker;

export default function Dashboard() {
  const [loading, setLoading] = useState(false);
  const [overview, setOverview] = useState<OverviewStatistics | null>(null);
  const [topProducts, setTopProducts] = useState<ProductStatistics[]>([]);
  const [activeEmployees, setActiveEmployees] = useState<EmployeeStatistics[]>([]);
  const [dateRange, setDateRange] = useState<[dayjs.Dayjs, dayjs.Dayjs]>([
    dayjs().subtract(30, 'days'),
    dayjs(),
  ]);

  useEffect(() => {
    loadStatistics();
  }, [dateRange]);

  const loadStatistics = async () => {
    try {
      setLoading(true);
      const params = {
        startDate: dateRange[0].toISOString(),
        endDate: dateRange[1].toISOString(),
        topN: 10,
      };

      const [overviewData, productsData, employeesData] = await Promise.all([
        getOverviewStatistics(params),
        getTopProducts(params),
        getActiveEmployees(params),
      ]);

      setOverview(overviewData);
      setTopProducts(productsData);
      setActiveEmployees(employeesData);
    } catch (error) {
      console.error('Failed to load statistics', error);
    } finally {
      setLoading(false);
    }
  };

  const productColumns = [
    {
      title: '排名',
      key: 'rank',
      width: 60,
      render: (_: any, __: any, index: number) => index + 1,
    },
    {
      title: '产品名称',
      dataIndex: 'productName',
      key: 'productName',
    },
    {
      title: '兑换次数',
      dataIndex: 'orderCount',
      key: 'orderCount',
      sorter: (a: ProductStatistics, b: ProductStatistics) => a.orderCount - b.orderCount,
    },
    {
      title: '总数量',
      dataIndex: 'totalQuantity',
      key: 'totalQuantity',
    },
    {
      title: '总积分',
      dataIndex: 'totalPoints',
      key: 'totalPoints',
      sorter: (a: ProductStatistics, b: ProductStatistics) => a.totalPoints - b.totalPoints,
    },
  ];

  const employeeColumns = [
    {
      title: '排名',
      key: 'rank',
      width: 60,
      render: (_: any, __: any, index: number) => index + 1,
    },
    {
      title: '员工姓名',
      dataIndex: 'employeeName',
      key: 'employeeName',
    },
    {
      title: '邮箱',
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: '订单数',
      dataIndex: 'orderCount',
      key: 'orderCount',
      sorter: (a: EmployeeStatistics, b: EmployeeStatistics) => a.orderCount - b.orderCount,
    },
    {
      title: '消费积分',
      dataIndex: 'totalPointsConsumed',
      key: 'totalPointsConsumed',
      sorter: (a: EmployeeStatistics, b: EmployeeStatistics) => 
        a.totalPointsConsumed - b.totalPointsConsumed,
    },
    {
      title: '最后订单',
      dataIndex: 'lastOrderDate',
      key: 'lastOrderDate',
      render: (date: string) => date ? dayjs(date).format('YYYY-MM-DD') : '-',
    },
  ];

  if (loading && !overview) {
    return (
      <div style={{ padding: 24, textAlign: 'center' }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      <Space direction="vertical" size="large" style={{ width: '100%' }}>
        <div className="dashboard-header">
          <h2>数据统计</h2>
          <RangePicker
            value={dateRange}
            onChange={(dates) => dates && setDateRange(dates as [dayjs.Dayjs, dayjs.Dayjs])}
            format="YYYY-MM-DD"
            style={{ width: '100%', maxWidth: 300 }}
          />
        </div>

        {/* 概览卡片 */}
        <Row gutter={[16, 16]}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="总订单数"
                value={overview?.totalOrders || 0}
                prefix={<ShoppingOutlined />}
                valueStyle={{ color: '#3f8600' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="活跃员工"
                value={overview?.activeEmployees || 0}
                suffix={`/ ${overview?.totalEmployees || 0}`}
                prefix={<UserOutlined />}
                valueStyle={{ color: '#1890ff' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="积分发放"
                value={overview?.totalPointsIssued || 0}
                prefix={<GiftOutlined />}
                valueStyle={{ color: '#cf1322' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="积分消费"
                value={overview?.totalPointsConsumed || 0}
                prefix={<TrophyOutlined />}
                valueStyle={{ color: '#faad14' }}
              />
            </Card>
          </Col>
        </Row>

        {/* 次要指标 */}
        <Row gutter={[16, 16]}>
          <Col xs={24} sm={8}>
            <Card size="small">
              <Statistic
                title="在线产品"
                value={overview?.onlineProducts || 0}
                suffix={`/ ${overview?.totalProducts || 0}`}
              />
            </Card>
          </Col>
          <Col xs={24} sm={8}>
            <Card size="small">
              <Statistic
                title="待处理订单"
                value={overview?.pendingOrders || 0}
                valueStyle={{ color: overview?.pendingOrders ? '#faad14' : undefined }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={8}>
            <Card size="small">
              <Statistic
                title="积分净变化"
                value={(overview?.totalPointsIssued || 0) - (overview?.totalPointsConsumed || 0)}
                valueStyle={{
                  color: ((overview?.totalPointsIssued || 0) - (overview?.totalPointsConsumed || 0)) >= 0
                    ? '#3f8600'
                    : '#cf1322',
                }}
              />
            </Card>
          </Col>
        </Row>

        {/* 热门产品排行 */}
        <Card title="热门产品排行" loading={loading}>
          <Table
            columns={productColumns}
            dataSource={topProducts}
            rowKey="productId"
            pagination={false}
            size="small"
            scroll={{ x: 800 }}
          />
        </Card>

        {/* 活跃员工排行 */}
        <Card title="活跃员工排行" loading={loading}>
          <Table
            columns={employeeColumns}
            dataSource={activeEmployees}
            rowKey="employeeId"
            pagination={false}
            size="small"
            scroll={{ x: 900 }}
          />
        </Card>
      </Space>
    </div>
  );
}
