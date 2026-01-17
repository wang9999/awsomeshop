import { create } from 'zustand';
import { getPointsBalance, type PointsBalance } from '../services/pointsService';

interface PointsState {
  balance: number;
  expiringPoints: number;
  loading: boolean;
  error: string | null;

  // Actions
  fetchBalance: () => Promise<void>;
  updateBalance: (balance: number) => void;
  reset: () => void;
}

export const usePointsStore = create<PointsState>((set) => ({
  balance: 0,
  expiringPoints: 0,
  loading: false,
  error: null,

  fetchBalance: async () => {
    set({ loading: true, error: null });
    try {
      const data = await getPointsBalance();
      set({ 
        balance: data.balance, 
        expiringPoints: data.expiringPoints,
        loading: false 
      });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : 'Failed to fetch balance',
        loading: false 
      });
    }
  },

  updateBalance: (balance: number) => {
    set({ balance });
  },

  reset: () => {
    set({ balance: 0, expiringPoints: 0, loading: false, error: null });
  },
}));
