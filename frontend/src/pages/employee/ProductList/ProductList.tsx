import React, { useState, useEffect } from 'react';
import { Row, Col, Input, Select, Slider, Card, Spin, Empty, Pagination, Space } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import ProductCard from '../../../components/ProductCard/ProductCard';
import { getProducts, type Product, type ProductQueryParams } from '../../../services/productService';
import './ProductList.css';

const { Option } = Select;

const ProductList: React.FC = () => {
  const navigate = useNavigate();
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 12,
    total: 0,
  });
  const [filters, setFilters] = useState<ProductQueryParams>({
    pageNumber: 1,
    pageSize: 12,
    search: '',
    category: undefined,
    minPoints: undefined,
    maxPoints: undefined,
    inStock: undefined,
  });
  const [pointsRange, setPointsRange] = useState<[number, number]>([0, 10000]);

  const fetchProducts = async (params: ProductQueryParams) => {
    setLoading(true);
    try {
      const data = await getProducts(params);
      setProducts(data.products);
      setPagination({
        current: data.pageNumber,
        pageSize: data.pageSize,
        total: data.totalCount,
      });
    } catch (error) {
      console.error('Failed to fetch products:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts(filters);
  }, []);

  const handleSearch = (value: string) => {
    const newFilters = { ...filters, search: value, pageNumber: 1 };
    setFilters(newFilters);
    fetchProducts(newFilters);
  };

  const handleCategoryChange = (value: string) => {
    const newFilters = { ...filters, category: value || undefined, pageNumber: 1 };
    setFilters(newFilters);
    fetchProducts(newFilters);
  };

  const handleStockChange = (value: string) => {
    const inStock = value === 'inStock' ? true : value === 'outOfStock' ? false : undefined;
    const newFilters = { ...filters, inStock, pageNumber: 1 };
    setFilters(newFilters);
    fetchProducts(newFilters);
  };

  const handlePointsRangeChange = (value: number | number[]) => {
    if (Array.isArray(value)) {
      setPointsRange(value as [number, number]);
    }
  };

  const handlePointsRangeAfterChange = (value: number | number[]) => {
    if (Array.isArray(value)) {
      const newFilters = {
        ...filters,
        minPoints: value[0],
        maxPoints: value[1],
        pageNumber: 1,
      };
      setFilters(newFilters);
      fetchProducts(newFilters);
    }
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    const newFilters = { ...filters, pageNumber: page, pageSize: pageSize || filters.pageSize };
    setFilters(newFilters);
    fetchProducts(newFilters);
  };

  const handleProductClick = (productId: string) => {
    navigate(`/products/${productId}`);
  };

  return (
    <div className="product-list-container">
      <Card className="filter-card">
        <Space direction="vertical" style={{ width: '100%' }} size="middle">
          <Input
            placeholder="搜索产品名称或描述"
            prefix={<SearchOutlined />}
            size="large"
            onPressEnter={(e) => handleSearch(e.currentTarget.value)}
            allowClear
          />
          
          <Row gutter={[16, 16]}>
            <Col xs={24} sm={12} md={8}>
              <div className="filter-label">分类</div>
              <Select
                style={{ width: '100%' }}
                placeholder="全部分类"
                onChange={handleCategoryChange}
                allowClear
              >
                <Option value="电子产品">电子产品</Option>
                <Option value="生活用品">生活用品</Option>
                <Option value="礼品卡">礼品卡</Option>
                <Option value="图书文具">图书文具</Option>
              </Select>
            </Col>
            
            <Col xs={24} sm={12} md={8}>
              <div className="filter-label">库存状态</div>
              <Select
                style={{ width: '100%' }}
                placeholder="全部"
                onChange={handleStockChange}
                allowClear
              >
                <Option value="inStock">有货</Option>
                <Option value="outOfStock">缺货</Option>
              </Select>
            </Col>
            
            <Col xs={24} sm={24} md={8}>
              <div className="filter-label">积分范围: {pointsRange[0]} - {pointsRange[1]}</div>
              <Slider
                range
                min={0}
                max={10000}
                step={100}
                value={pointsRange}
                onChange={handlePointsRangeChange}
                onChangeComplete={handlePointsRangeAfterChange}
              />
            </Col>
          </Row>
        </Space>
      </Card>

      {loading ? (
        <div style={{ textAlign: 'center', padding: '50px' }}>
          <Spin size="large" />
        </div>
      ) : products.length === 0 ? (
        <Empty description="暂无产品" style={{ marginTop: '50px' }} />
      ) : (
        <>
          <Row gutter={[16, 16]} style={{ marginTop: '24px' }}>
            {products.map((product) => (
              <Col key={product.id} xs={24} sm={12} md={8} lg={6}>
                <ProductCard product={product} onClick={() => handleProductClick(product.id)} />
              </Col>
            ))}
          </Row>

          <div style={{ textAlign: 'center', marginTop: '32px' }}>
            <Pagination
              current={pagination.current}
              pageSize={pagination.pageSize}
              total={pagination.total}
              onChange={handlePageChange}
              showSizeChanger
              showTotal={(total) => `共 ${total} 个产品`}
            />
          </div>
        </>
      )}
    </div>
  );
};

export default ProductList;
