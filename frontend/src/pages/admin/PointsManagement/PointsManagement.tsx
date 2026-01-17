import React, { useState } from 'react';
import { Card, Table, Button, Modal, Form, Input, InputNumber, DatePicker, message, Space, Tag, Select } from 'antd';
import { PlusOutlined, MinusOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { grantPoints, deductPoints, batchGrantPoints } from '../../../services/pointsService';

const { TextArea } = Input;
const { Option } = Select;

interface Employee {
  id: string;
  name: string;
  email: string;
  employeeNumber: string;
  department?: string;
  pointsBalance: number;
  isActive: boolean;
}

const PointsManagement: React.FC = () => {
  const [employees] = useState<Employee[]>([
    // Mock data - in real app, fetch from API
    {
      id: '1',
      name: '张三',
      email: 'zhangsan@company.com',
      employeeNumber: 'EMP001',
      department: '技术部',
      pointsBalance: 1500,
      isActive: true,
    },
    {
      id: '2',
      name: '李四',
      email: 'lisi@company.com',
      employeeNumber: 'EMP002',
      department: '市场部',
      pointsBalance: 2000,
      isActive: true,
    },
  ]);
  const [selectedEmployees, setSelectedEmployees] = useState<string[]>([]);
  const [grantModalVisible, setGrantModalVisible] = useState(false);
  const [deductModalVisible, setDeductModalVisible] = useState(false);
  const [batchGrantModalVisible, setBatchGrantModalVisible] = useState(false);
  const [currentEmployee, setCurrentEmployee] = useState<Employee | null>(null);
  const [loading, setLoading] = useState(false);
  const [grantForm] = Form.useForm();
  const [deductForm] = Form.useForm();
  const [batchGrantForm] = Form.useForm();

  const handleGrant = (employee: Employee) => {
    setCurrentEmployee(employee);
    setGrantModalVisible(true);
    grantForm.resetFields();
  };

  const handleDeduct = (employee: Employee) => {
    setCurrentEmployee(employee);
    setDeductModalVisible(true);
    deductForm.resetFields();
  };

  const handleBatchGrant = () => {
    if (selectedEmployees.length === 0) {
      message.warning('请先选择员工');
      return;
    }
    setBatchGrantModalVisible(true);
    batchGrantForm.resetFields();
  };

  const onGrantSubmit = async (values: any) => {
    if (!currentEmployee) return;

    setLoading(true);
    try {
      await grantPoints({
        employeeId: currentEmployee.id,
        amount: values.amount,
        reason: values.reason,
        expiryDate: values.expiryDate?.format('YYYY-MM-DD'),
      });
      message.success('积分发放成功');
      setGrantModalVisible(false);
      // Refresh employee list
    } catch (error) {
      message.error('积分发放失败');
    } finally {
      setLoading(false);
    }
  };

  const onDeductSubmit = async (values: any) => {
    if (!currentEmployee) return;

    setLoading(true);
    try {
      await deductPoints({
        employeeId: currentEmployee.id,
        amount: values.amount,
        reason: values.reason,
      });
      message.success('积分扣除成功');
      setDeductModalVisible(false);
      // Refresh employee list
    } catch (error: any) {
      message.error(error.response?.data?.message || '积分扣除失败');
    } finally {
      setLoading(false);
    }
  };

  const onBatchGrantSubmit = async (values: any) => {
    setLoading(true);
    try {
      const result = await batchGrantPoints({
        employeeIds: selectedEmployees,
        amount: values.amount,
        reason: values.reason,
        expiryDate: values.expiryDate?.format('YYYY-MM-DD'),
      });
      
      if (result.failedEmployees.length > 0) {
        message.warning(`成功发放 ${result.successCount}/${result.totalCount} 个员工，${result.failedEmployees.length} 个失败`);
      } else {
        message.success(`成功发放 ${result.successCount} 个员工`);
      }
      
      setBatchGrantModalVisible(false);
      setSelectedEmployees([]);
      // Refresh employee list
    } catch (error) {
      message.error('批量发放失败');
    } finally {
      setLoading(false);
    }
  };

  const columns: ColumnsType<Employee> = [
    {
      title: '员工编号',
      dataIndex: 'employeeNumber',
      key: 'employeeNumber',
      width: 120,
    },
    {
      title: '姓名',
      dataIndex: 'name',
      key: 'name',
      width: 120,
    },
    {
      title: '邮箱',
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: '部门',
      dataIndex: 'department',
      key: 'department',
      width: 120,
    },
    {
      title: '积分余额',
      dataIndex: 'pointsBalance',
      key: 'pointsBalance',
      width: 120,
      render: (balance: number) => (
        <span style={{ fontWeight: 'bold', color: '#ff9900' }}>{balance}</span>
      ),
    },
    {
      title: '状态',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (isActive: boolean) => (
        <Tag color={isActive ? 'green' : 'red'}>
          {isActive ? '在职' : '离职'}
        </Tag>
      ),
    },
    {
      title: '操作',
      key: 'action',
      width: 180,
      fixed: 'right',
      render: (_, record) => (
        <Space>
          <Button 
            type="link" 
            icon={<PlusOutlined />}
            onClick={() => handleGrant(record)}
          >
            发放
          </Button>
          <Button 
            type="link" 
            danger 
            icon={<MinusOutlined />}
            onClick={() => handleDeduct(record)}
          >
            扣除
          </Button>
        </Space>
      ),
    },
  ];

  const rowSelection = {
    selectedRowKeys: selectedEmployees,
    onChange: (selectedRowKeys: React.Key[]) => {
      setSelectedEmployees(selectedRowKeys as string[]);
    },
  };

  return (
    <div>
      <Card 
        title="积分管理" 
        extra={
          <Space>
            <Input
              placeholder="搜索员工姓名或编号"
              prefix={<SearchOutlined />}
              style={{ width: 250 }}
            />
            <Button 
              type="primary" 
              icon={<PlusOutlined />}
              onClick={handleBatchGrant}
              disabled={selectedEmployees.length === 0}
            >
              批量发放 ({selectedEmployees.length})
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={employees}
          rowKey="id"
          rowSelection={rowSelection}
          scroll={{ x: 1000 }}
        />
      </Card>

      {/* Grant Points Modal */}
      <Modal
        title={`发放积分 - ${currentEmployee?.name}`}
        open={grantModalVisible}
        onCancel={() => setGrantModalVisible(false)}
        onOk={() => grantForm.submit()}
        confirmLoading={loading}
      >
        <Form form={grantForm} layout="vertical" onFinish={onGrantSubmit}>
          <Form.Item
            label="发放积分"
            name="amount"
            rules={[
              { required: true, message: '请输入发放积分' },
              { type: 'number', min: 1, message: '积分必须大于0' },
            ]}
          >
            <InputNumber style={{ width: '100%' }} placeholder="请输入发放积分" />
          </Form.Item>
          <Form.Item
            label="发放原因"
            name="reason"
            rules={[{ required: true, message: '请输入发放原因' }]}
          >
            <TextArea rows={3} placeholder="请输入发放原因" />
          </Form.Item>
          <Form.Item
            label="过期时间"
            name="expiryDate"
            tooltip="不填写则默认1年后过期"
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>

      {/* Deduct Points Modal */}
      <Modal
        title={`扣除积分 - ${currentEmployee?.name}`}
        open={deductModalVisible}
        onCancel={() => setDeductModalVisible(false)}
        onOk={() => deductForm.submit()}
        confirmLoading={loading}
      >
        <Form form={deductForm} layout="vertical" onFinish={onDeductSubmit}>
          <div style={{ marginBottom: 16, padding: 12, background: '#f5f5f5', borderRadius: 4 }}>
            当前余额: <span style={{ fontWeight: 'bold', color: '#ff9900' }}>{currentEmployee?.pointsBalance}</span> 积分
          </div>
          <Form.Item
            label="扣除积分"
            name="amount"
            rules={[
              { required: true, message: '请输入扣除积分' },
              { type: 'number', min: 1, message: '积分必须大于0' },
              {
                validator: (_, value) => {
                  if (currentEmployee && value > currentEmployee.pointsBalance) {
                    return Promise.reject('扣除积分不能超过当前余额');
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <InputNumber style={{ width: '100%' }} placeholder="请输入扣除积分" />
          </Form.Item>
          <Form.Item
            label="扣除原因"
            name="reason"
            rules={[{ required: true, message: '请输入扣除原因' }]}
          >
            <TextArea rows={3} placeholder="请输入扣除原因" />
          </Form.Item>
        </Form>
      </Modal>

      {/* Batch Grant Points Modal */}
      <Modal
        title={`批量发放积分 (${selectedEmployees.length} 个员工)`}
        open={batchGrantModalVisible}
        onCancel={() => setBatchGrantModalVisible(false)}
        onOk={() => batchGrantForm.submit()}
        confirmLoading={loading}
        width={600}
      >
        <Form form={batchGrantForm} layout="vertical" onFinish={onBatchGrantSubmit}>
          <Form.Item
            label="发放积分"
            name="amount"
            rules={[
              { required: true, message: '请输入发放积分' },
              { type: 'number', min: 1, message: '积分必须大于0' },
            ]}
          >
            <InputNumber style={{ width: '100%' }} placeholder="请输入发放积分" />
          </Form.Item>
          <Form.Item
            label="发放原因"
            name="reason"
            rules={[{ required: true, message: '请输入发放原因' }]}
          >
            <TextArea rows={3} placeholder="请输入发放原因" />
          </Form.Item>
          <Form.Item
            label="过期时间"
            name="expiryDate"
            tooltip="不填写则默认1年后过期"
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default PointsManagement;
