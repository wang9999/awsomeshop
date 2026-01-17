import { get, post, put, del } from '../utils/request';

export interface Product {
  id: string;
  name: string;
  description?: string;
  points: number;
  stock: number;
  category: string;
  status: string;
  images: string[];
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface ProductListResponse {
  products: Product[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface ProductQueryParams {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  category?: string;
  minPoints?: number;
  maxPoints?: number;
  inStock?: boolean;
  status?: string;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  points: number;
  stock: number;
  category: string;
  imageUrls?: string[];
  tags?: string[];
}

export interface UpdateProductRequest {
  name: string;
  description?: string;
  points: number;
  stock: number;
  category: string;
  imageUrls?: string[];
  tags?: string[];
}

/**
 * Get product list (Employee - only online products)
 */
export const getProducts = (params?: ProductQueryParams): Promise<ProductListResponse> => {
  return get<ProductListResponse>('/products', params);
};

/**
 * Get product details (Employee)
 */
export const getProductById = (id: string): Promise<Product> => {
  return get<Product>(`/products/${id}`);
};

/**
 * Admin: Get all products
 */
export const getProductsAdmin = (params?: ProductQueryParams): Promise<ProductListResponse> => {
  return get<ProductListResponse>('/admin/products', params);
};

/**
 * Admin: Get product details
 */
export const getProductByIdAdmin = (id: string): Promise<Product> => {
  return get<Product>(`/admin/products/${id}`);
};

/**
 * Admin: Create product
 */
export const createProduct = (data: CreateProductRequest): Promise<Product> => {
  return post<Product>('/admin/products', data);
};

/**
 * Admin: Update product
 */
export const updateProduct = (id: string, data: UpdateProductRequest): Promise<Product> => {
  return put<Product>(`/admin/products/${id}`, data);
};

/**
 * Admin: Delete product
 */
export const deleteProduct = (id: string): Promise<{ message: string }> => {
  return del<{ message: string }>(`/admin/products/${id}`);
};

/**
 * Admin: Update product status
 */
export const updateProductStatus = (id: string, status: string): Promise<Product> => {
  return put<Product>(`/admin/products/${id}/status`, { status });
};
