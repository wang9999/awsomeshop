import React from 'react';
import { Card, Tag, Badge } from 'antd';
import { ShoppingCartOutlined } from '@ant-design/icons';
import type { Product } from '../../services/productService';
import './ProductCard.css';

const { Meta } = Card;

interface ProductCardProps {
  product: Product;
  onClick?: () => void;
}

const ProductCard: React.FC<ProductCardProps> = ({ product, onClick }) => {
  const isOutOfStock = product.stock === 0;
  const coverImage = product.images && product.images.length > 0 
    ? product.images[0] 
    : 'https://via.placeholder.com/300x200?text=No+Image';

  return (
    <Badge.Ribbon 
      text={isOutOfStock ? '缺货' : '有货'} 
      color={isOutOfStock ? 'red' : 'green'}
    >
      <Card
        hoverable
        className="product-card"
        cover={
          <div className="product-card-cover">
            <img alt={product.name} src={coverImage} />
          </div>
        }
        onClick={onClick}
      >
        <Meta
          title={
            <div className="product-card-title">
              {product.name}
            </div>
          }
          description={
            <div className="product-card-description">
              {product.description && product.description.length > 50
                ? `${product.description.substring(0, 50)}...`
                : product.description}
            </div>
          }
        />
        <div className="product-card-footer">
          <div className="product-card-points">
            <ShoppingCartOutlined style={{ marginRight: '4px' }} />
            <span className="points-value">{product.points}</span>
            <span className="points-label"> 积分</span>
          </div>
          <div className="product-card-tags">
            <Tag color="blue">{product.category}</Tag>
            {product.tags && product.tags.slice(0, 2).map((tag, index) => (
              <Tag key={index}>{tag}</Tag>
            ))}
          </div>
        </div>
      </Card>
    </Badge.Ribbon>
  );
};

export default ProductCard;
