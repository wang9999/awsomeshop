import { create } from 'zustand';
import { getCart, addToCart, updateCartItem, removeFromCart, clearCart, type CartItem } from '../services/cartService';

interface CartState {
  items: CartItem[];
  totalPoints: number;
  loading: boolean;
  error: string | null;

  // Actions
  fetchCart: () => Promise<void>;
  addItem: (productId: string, quantity: number) => Promise<void>;
  updateQuantity: (itemId: string, quantity: number) => Promise<void>;
  removeItem: (itemId: string) => Promise<void>;
  clear: () => Promise<void>;
  reset: () => void;
}

export const useCartStore = create<CartState>((set) => ({
  items: [],
  totalPoints: 0,
  loading: false,
  error: null,

  fetchCart: async () => {
    set({ loading: true, error: null });
    try {
      const cart = await getCart();
      set({ 
        items: cart.items, 
        totalPoints: cart.totalPoints,
        loading: false 
      });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : 'Failed to fetch cart',
        loading: false 
      });
    }
  },

  addItem: async (productId: string, quantity: number) => {
    set({ loading: true, error: null });
    try {
      const cart = await addToCart({ productId, quantity });
      set({ 
        items: cart.items, 
        totalPoints: cart.totalPoints,
        loading: false 
      });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : 'Failed to add item',
        loading: false 
      });
      throw error;
    }
  },

  updateQuantity: async (itemId: string, quantity: number) => {
    set({ loading: true, error: null });
    try {
      const cart = await updateCartItem(itemId, { quantity });
      set({ 
        items: cart.items, 
        totalPoints: cart.totalPoints,
        loading: false 
      });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : 'Failed to update quantity',
        loading: false 
      });
      throw error;
    }
  },

  removeItem: async (itemId: string) => {
    set({ loading: true, error: null });
    try {
      const cart = await removeFromCart(itemId);
      set({ 
        items: cart.items, 
        totalPoints: cart.totalPoints,
        loading: false 
      });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : 'Failed to remove item',
        loading: false 
      });
      throw error;
    }
  },

  clear: async () => {
    set({ loading: true, error: null });
    try {
      await clearCart();
      set({ 
        items: [], 
        totalPoints: 0,
        loading: false 
      });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : 'Failed to clear cart',
        loading: false 
      });
      throw error;
    }
  },

  reset: () => {
    set({ items: [], totalPoints: 0, loading: false, error: null });
  },
}));
