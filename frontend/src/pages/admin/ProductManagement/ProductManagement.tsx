import React, { useState, useEffect } from 'react';
import { Card, Table, Button, Modal, Form, Input, InputNumber, Select, Tag, Space, message, Switch } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import {
  getProductsAdmin,
  createProduct,
  updateProduct,
  deleteProduct,
  updateProductStatus,
  type Product,
  type CreateProductRequest,
  type UpdateProductRequest,
} from '../../../services/productService';

const { TextArea } = Input;
const { Option } = Select;

const ProductManagement: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [form] = Form.useForm();

  const fetchProducts = async (page = 1, pageSize = 10) => {
    setLoading(true);
    try {
      const data = await getProductsAdmin({ pageNumber: page, pageSize });
      setProducts(data.products);
      setPagination({
        current: data.pageNumber,
        pageSize: data.pageSize,
        total: data.totalCount,
      });
    } catch (error) {
      message.error('加载产品失败');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const handleCreate = () => {
    setEditingProduct(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (product: Product) => {
    setEditingProduct(product);
    form.setFieldsValue({
      name: product.name,
      description: product.description,
      points: product.points,
      stock: product.stock,
      category: product.category,
      imageUrls: product.images.join('\n'),
      tags: product.tags.join(', '),
    });
    setModalVisible(true);
  };

  const handleDelete = async (id: string) => {
    Modal.confirm({
      title: '确认删除',
      content: '确定要删除这个产品吗？',
      onOk: async () => {
        try {
          await deleteProduct(id);
          message.success('删除成功');
          fetchProducts(pagination.current, pagination.pageSize);
        } catch (error) {
          message.error('删除失败');
        }
      },
    });
  };

  const handleStatusChange = async (id: string, checked: boolean) => {
    try {
      await updateProductStatus(id, checked ? '上架' : '下架');
      message.success('状态更新成功');
      fetchProducts(pagination.current, pagination.pageSize);
    } catch (error) {
      message.error('状态更新失败');
    }
  };

  const handleSubmit = async (values: any) => {
    try {
      const imageUrls = values.imageUrls
        ? values.imageUrls.split('\n').map((url: string) => url.trim()).filter(Boolean)
        : [];
      const tags = values.tags
        ? values.tags.split(',').map((tag: string) => tag.trim()).filter(Boolean)
        : [];

      const productData = {
        name: values.name,
        description: values.description,
        points: values.points,
        stock: values.stock,
        category: values.category,
        imageUrls,
        tags,
      };

      if (editingProduct) {
        await updateProduct(editingProduct.id, productData as UpdateProductRequest);
        message.success('更新成功');
      } else {
        await createProduct(productData as CreateProductRequest);
        message.success('创建成功');
      }

      setModalVisible(false);
      fetchProducts(pagination.current, pagination.pageSize);
    } catch (error) {
      message.error(editingProduct ? '更新失败' : '创建失败');
    }
  };

  const columns: ColumnsType<Product> = [
    {
      title: '产品名称',
      dataIndex: 'name',
      key: 'name',
      width: 200,
    },
    {
      title: '分类',
      dataIndex: 'category',
      key: 'category',
      width: 100,
      render: (category: string) => <Tag color="blue">{category}</Tag>,
    },
    {
      title: '积分',
      dataIndex: 'points',
      key: 'points',
      width: 100,
      render: (points: number) => (
        <span style={{ color: '#ff9900', fontWeight: 'bold' }}>{points}</span>
      ),
    },
    {
      title: '库存',
      dataIndex: 'stock',
      key: 'stock',
      width: 80,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (status: string, record: Product) => (
        <Switch
          checked={status === '上架'}
          onChange={(checked) => handleStatusChange(record.id, checked)}
          checkedChildren="上架"
          unCheckedChildren="下架"
        />
      ),
    },
    {
      title: '标签',
      dataIndex: 'tags',
      key: 'tags',
      width: 150,
      render: (tags: string[]) => (
        <>
          {tags && tags.slice(0, 2).map((tag, index) => (
            <Tag key={index}>{tag}</Tag>
          ))}
        </>
      ),
    },
    {
      title: '操作',
      key: 'action',
      width: 150,
      fixed: 'right',
      render: (_, record) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            编辑
          </Button>
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() => handleDelete(record.id)}
          >
            删除
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <Card
        title="产品管理"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
            新建产品
          </Button>
        }
      >
        <Table
          columns={columns}
          dataSource={products}
          rowKey="id"
          loading={loading}
          pagination={pagination}
          onChange={(newPagination) => {
            fetchProducts(newPagination.current, newPagination.pageSize);
          }}
          scroll={{ x: 1000 }}
        />
      </Card>

      <Modal
        title={editingProduct ? '编辑产品' : '新建产品'}
        open={modalVisible}
        onCancel={() => setModalVisible(false)}
        onOk={() => form.submit()}
        width={600}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item
            label="产品名称"
            name="name"
            rules={[{ required: true, message: '请输入产品名称' }]}
          >
            <Input placeholder="请输入产品名称" />
          </Form.Item>

          <Form.Item label="产品描述" name="description">
            <TextArea rows={4} placeholder="请输入产品描述" />
          </Form.Item>

          <Form.Item
            label="所需积分"
            name="points"
            rules={[{ required: true, message: '请输入所需积分' }]}
          >
            <InputNumber min={0} style={{ width: '100%' }} placeholder="请输入所需积分" />
          </Form.Item>

          <Form.Item
            label="库存数量"
            name="stock"
            rules={[{ required: true, message: '请输入库存数量' }]}
          >
            <InputNumber min={0} style={{ width: '100%' }} placeholder="请输入库存数量" />
          </Form.Item>

          <Form.Item
            label="产品分类"
            name="category"
            rules={[{ required: true, message: '请选择产品分类' }]}
          >
            <Select placeholder="请选择产品分类">
              <Option value="电子产品">电子产品</Option>
              <Option value="生活用品">生活用品</Option>
              <Option value="礼品卡">礼品卡</Option>
              <Option value="图书文具">图书文具</Option>
            </Select>
          </Form.Item>

          <Form.Item label="产品图片" name="imageUrls" tooltip="每行一个图片URL">
            <TextArea rows={3} placeholder="每行输入一个图片URL" />
          </Form.Item>

          <Form.Item label="产品标签" name="tags" tooltip="多个标签用逗号分隔">
            <Input placeholder="例如: 热门, 新品, 限量" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default ProductManagement;
