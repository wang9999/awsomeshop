import { get, post, put, del } from '../utils/request';

export interface CartItem {
  id: string;
  productId: string;
  productName: string;
  productImage?: string;
  points: number;
  quantity: number;
  stock: number;
}

export interface ShoppingCart {
  items: CartItem[];
  totalPoints: number;
}

export interface AddCartItemRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}

/**
 * Get shopping cart
 */
export const getCart = (): Promise<ShoppingCart> => {
  return get<ShoppingCart>('/cart');
};

/**
 * Add item to cart
 */
export const addToCart = (data: AddCartItemRequest): Promise<ShoppingCart> => {
  return post<ShoppingCart>('/cart/items', data);
};

/**
 * Update cart item quantity
 */
export const updateCartItem = (id: string, data: UpdateCartItemRequest): Promise<ShoppingCart> => {
  return put<ShoppingCart>(`/cart/items/${id}`, data);
};

/**
 * Remove item from cart
 */
export const removeFromCart = (id: string): Promise<ShoppingCart> => {
  return del<ShoppingCart>(`/cart/items/${id}`);
};

/**
 * Clear cart
 */
export const clearCart = (): Promise<{ message: string }> => {
  return del<{ message: string }>('/cart');
};
