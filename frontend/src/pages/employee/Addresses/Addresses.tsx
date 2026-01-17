import { useState, useEffect } from 'react';
import { Button, Card, List, Tag, message, Modal, Form, Input, Switch } from 'antd';
import { useAddressStore } from '../../../store/addressSlice';
import {
  getAddresses,
  createAddress,
  updateAddress,
  deleteAddress,
  setDefaultAddress,
  Address,
  CreateAddressRequest,
} from '../../../services/addressService';

export default function Addresses() {
  const { addresses, setAddresses, setLoading } = useAddressStore();
  const [modalVisible, setModalVisible] = useState(false);
  const [editingAddress, setEditingAddress] = useState<Address | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    loadAddresses();
  }, []);

  const loadAddresses = async () => {
    try {
      setLoading(true);
      const data = await getAddresses();
      setAddresses(data);
    } catch (error) {
      message.error('加载地址失败');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (values: CreateAddressRequest) => {
    try {
      if (editingAddress) {
        await updateAddress(editingAddress.id, values);
        message.success('地址更新成功');
      } else {
        await createAddress(values);
        message.success('地址添加成功');
      }
      setModalVisible(false);
      form.resetFields();
      setEditingAddress(null);
      loadAddresses();
    } catch (error) {
      message.error('操作失败');
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await deleteAddress(id);
      message.success('地址删除成功');
      loadAddresses();
    } catch (error) {
      message.error('删除失败');
    }
  };

  const handleSetDefault = async (id: string) => {
    try {
      await setDefaultAddress(id);
      message.success('默认地址设置成功');
      loadAddresses();
    } catch (error) {
      message.error('设置失败');
    }
  };

  const openModal = (address?: Address) => {
    if (address) {
      setEditingAddress(address);
      form.setFieldsValue(address);
    } else {
      setEditingAddress(null);
      form.resetFields();
    }
    setModalVisible(true);
  };

  return (
    <div style={{ padding: 24, maxWidth: 800, margin: '0 auto' }}>
      <Card
        title="收货地址管理"
        extra={<Button type="primary" onClick={() => openModal()}>添加地址</Button>}
      >
        <List
          dataSource={addresses}
          renderItem={(addr) => (
            <List.Item
              actions={[
                !addr.isDefault && (
                  <Button type="link" onClick={() => handleSetDefault(addr.id)}>
                    设为默认
                  </Button>
                ),
                <Button type="link" onClick={() => openModal(addr)}>编辑</Button>,
                <Button type="link" danger onClick={() => handleDelete(addr.id)}>删除</Button>,
              ]}
            >
              <List.Item.Meta
                title={
                  <span>
                    {addr.receiverName} {addr.receiverPhone}
                    {addr.isDefault && <Tag color="green" style={{ marginLeft: 8 }}>默认</Tag>}
                  </span>
                }
                description={`${addr.province || ''} ${addr.city || ''} ${addr.district || ''} ${addr.detailAddress}`}
              />
            </List.Item>
          )}
        />
      </Card>

      <Modal
        title={editingAddress ? '编辑地址' : '添加地址'}
        open={modalVisible}
        onCancel={() => {
          setModalVisible(false);
          setEditingAddress(null);
          form.resetFields();
        }}
        onOk={() => form.submit()}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item name="receiverName" label="收货人" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="receiverPhone" label="手机号" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="province" label="省份">
            <Input />
          </Form.Item>
          <Form.Item name="city" label="城市">
            <Input />
          </Form.Item>
          <Form.Item name="district" label="区县">
            <Input />
          </Form.Item>
          <Form.Item name="detailAddress" label="详细地址" rules={[{ required: true }]}>
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item name="isDefault" label="设为默认" valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
