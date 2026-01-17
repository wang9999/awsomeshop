import { create } from 'zustand';
import { Order } from '../services/orderService';

interface OrderState {
  orders: Order[];
  currentOrder: Order | null;
  loading: boolean;
  
  // Actions
  setOrders: (orders: Order[]) => void;
  setCurrentOrder: (order: Order | null) => void;
  setLoading: (loading: boolean) => void;
  clearOrders: () => void;
}

export const useOrderStore = create<OrderState>((set) => ({
  orders: [],
  currentOrder: null,
  loading: false,

  setOrders: (orders: Order[]) => {
    set({ orders });
  },

  setCurrentOrder: (order: Order | null) => {
    set({ currentOrder: order });
  },

  setLoading: (loading: boolean) => {
    set({ loading });
  },

  clearOrders: () => {
    set({ orders: [], currentOrder: null });
  },
}));
