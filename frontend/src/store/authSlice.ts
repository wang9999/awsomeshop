import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User } from '../utils/auth';
import { setToken, setUser, clearAuth } from '../utils/auth';
import { employeeLogin, adminLogin } from '../services/authService';

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  
  // Actions
  login: (email: string, password: string) => Promise<void>;
  adminLogin: (username: string, password: string) => Promise<void>;
  logout: () => void;
  updateUser: (user: Partial<User>) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      loading: false,

      login: async (email: string, password: string) => {
        set({ loading: true });
        try {
          const response = await employeeLogin({ email, password });
          const user: User = {
            id: response.employeeId || '',
            name: response.name,
            email: response.email || email,
            role: 'employee',
            points: response.points || 0,
          };
          setToken(response.token);
          setUser(user);
          set({ token: response.token, user, isAuthenticated: true, loading: false });
        } catch (error) {
          set({ loading: false });
          throw error;
        }
      },

      adminLogin: async (username: string, password: string) => {
        set({ loading: true });
        try {
          const response = await adminLogin({ username, password });
          const user: User = {
            id: response.adminId || '',
            name: response.name,
            email: response.email || '',
            role: response.role || 'admin',
          };
          setToken(response.token);
          setUser(user);
          set({ token: response.token, user, isAuthenticated: true, loading: false });
        } catch (error) {
          set({ loading: false });
          throw error;
        }
      },

      logout: () => {
        clearAuth();
        set({ token: null, user: null, isAuthenticated: false });
      },

      updateUser: (userData: Partial<User>) => {
        set((state) => {
          if (!state.user) return state;
          const updatedUser = { ...state.user, ...userData };
          setUser(updatedUser);
          return { user: updatedUser };
        });
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);
