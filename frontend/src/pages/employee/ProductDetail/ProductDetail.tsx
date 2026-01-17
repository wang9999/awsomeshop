import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card, Row, Col, Button, Tag, Carousel, Spin, message, Descriptions, InputNumber } from 'antd';
import { ShoppingCartOutlined, LeftOutlined } from '@ant-design/icons';
import { getProductById, type Product } from '../../../services/productService';
import { usePointsStore } from '../../../store/pointsSlice';
import { useCartStore } from '../../../store/cartSlice';
import './ProductDetail.css';

const ProductDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [quantity, setQuantity] = useState(1);
  const { balance } = usePointsStore();
  const { addItem } = useCartStore();

  useEffect(() => {
    if (id) {
      fetchProduct(id);
    }
  }, [id]);

  const fetchProduct = async (productId: string) => {
    setLoading(true);
    try {
      const data = await getProductById(productId);
      setProduct(data);
    } catch (error) {
      message.error('加载产品失败');
      navigate('/products');
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = async () => {
    if (!product) return;
    
    if (product.stock === 0) {
      message.warning('该产品暂时缺货');
      return;
    }

    if (balance < product.points * quantity) {
      message.warning('积分不足');
      return;
    }

    try {
      await addItem(product.id, quantity);
      message.success('已添加到购物车');
      setQuantity(1);
    } catch (error) {
      message.error('添加失败，请重试');
    }
  };

  const handleBuyNow = () => {
    if (!product) return;
    
    if (product.stock === 0) {
      message.warning('该产品暂时缺货');
      return;
    }

    if (balance < product.points) {
      message.warning('积分不足');
      return;
    }

    // TODO: Navigate to checkout
    message.info('立即兑换功能开发中');
  };

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: '100px' }}>
        <Spin size="large" />
      </div>
    );
  }

  if (!product) {
    return null;
  }

  const isOutOfStock = product.stock === 0;
  const canAfford = balance >= (product.points * quantity);

  return (
    <div className="product-detail-container">
      <Button 
        icon={<LeftOutlined />} 
        onClick={() => navigate('/products')}
        style={{ marginBottom: '24px' }}
      >
        返回列表
      </Button>

      <Card>
        <Row gutter={[24, 24]}>
          <Col xs={24} md={12}>
            <div className="product-images">
              {product.images && product.images.length > 0 ? (
                <Carousel autoplay>
                  {product.images.map((image, index) => (
                    <div key={index} className="carousel-image">
                      <img src={image} alt={`${product.name} ${index + 1}`} />
                    </div>
                  ))}
                </Carousel>
              ) : (
                <div className="carousel-image">
                  <img src="https://via.placeholder.com/600x400?text=No+Image" alt={product.name} />
                </div>
              )}
            </div>
          </Col>

          <Col xs={24} md={12}>
            <div className="product-info">
              <h1 className="product-title">{product.name}</h1>
              
              <div className="product-tags">
                <Tag color="blue">{product.category}</Tag>
                {product.tags && product.tags.map((tag, index) => (
                  <Tag key={index}>{tag}</Tag>
                ))}
                {isOutOfStock && <Tag color="red">缺货</Tag>}
              </div>

              <div className="product-points">
                <span className="points-value">{product.points}</span>
                <span className="points-label"> 积分</span>
              </div>

              <Descriptions column={1} style={{ marginTop: '24px' }}>
                <Descriptions.Item label="库存">{product.stock} 件</Descriptions.Item>
                <Descriptions.Item label="分类">{product.category}</Descriptions.Item>
                <Descriptions.Item label="数量">
                  <InputNumber
                    min={1}
                    max={product.stock}
                    value={quantity}
                    onChange={(value) => setQuantity(value || 1)}
                    disabled={isOutOfStock}
                  />
                </Descriptions.Item>
                <Descriptions.Item label="我的积分余额">
                  <span style={{ color: canAfford ? '#52c41a' : '#ff4d4f', fontWeight: 'bold' }}>
                    {balance}
                  </span>
                </Descriptions.Item>
              </Descriptions>

              {!canAfford && !isOutOfStock && (
                <div className="insufficient-points-warning">
                  <Tag color="warning">积分不足，还需 {(product.points * quantity) - balance} 积分</Tag>
                </div>
              )}

              <div className="product-actions">
                <Button
                  type="default"
                  size="large"
                  icon={<ShoppingCartOutlined />}
                  onClick={handleAddToCart}
                  disabled={isOutOfStock || !canAfford}
                  style={{ marginRight: '16px' }}
                >
                  加入购物车
                </Button>
                <Button
                  type="primary"
                  size="large"
                  onClick={handleBuyNow}
                  disabled={isOutOfStock || !canAfford}
                >
                  立即兑换
                </Button>
              </div>

              {product.description && (
                <div className="product-description">
                  <h3>产品描述</h3>
                  <p>{product.description}</p>
                </div>
              )}
            </div>
          </Col>
        </Row>
      </Card>
    </div>
  );
};

export default ProductDetail;
