import { create } from 'zustand';

export interface Product {
  id: string;
  name: string;
  description: string;
  images: string[];
  points: number;
  stock: number;
  category: string;
  status: string;
  tags?: string[];
}

interface ProductState {
  products: Product[];
  currentProduct: Product | null;
  loading: boolean;
  
  // Actions
  setProducts: (products: Product[]) => void;
  setCurrentProduct: (product: Product | null) => void;
  setLoading: (loading: boolean) => void;
}

export const useProductStore = create<ProductState>((set) => ({
  products: [],
  currentProduct: null,
  loading: false,

  setProducts: (products: Product[]) => {
    set({ products });
  },

  setCurrentProduct: (product: Product | null) => {
    set({ currentProduct: product });
  },

  setLoading: (loading: boolean) => {
    set({ loading });
  },
}));
