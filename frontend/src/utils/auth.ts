// Authentication utility functions

export interface User {
  id: string;
  name: string;
  email?: string;
  role?: string;
  points?: number;
}

/**
 * Get stored token
 */
export const getToken = (): string | null => {
  return localStorage.getItem('token');
};

/**
 * Set token
 */
export const setToken = (token: string): void => {
  localStorage.setItem('token', token);
};

/**
 * Remove token
 */
export const removeToken = (): void => {
  localStorage.removeItem('token');
};

/**
 * Get stored user info
 */
export const getUser = (): User | null => {
  const userStr = localStorage.getItem('user');
  if (userStr) {
    try {
      return JSON.parse(userStr);
    } catch {
      return null;
    }
  }
  return null;
};

/**
 * Set user info
 */
export const setUser = (user: User): void => {
  localStorage.setItem('user', JSON.stringify(user));
};

/**
 * Remove user info
 */
export const removeUser = (): void => {
  localStorage.removeItem('user');
};

/**
 * Check if user is authenticated
 */
export const isAuthenticated = (): boolean => {
  return !!getToken();
};

/**
 * Clear all auth data
 */
export const clearAuth = (): void => {
  removeToken();
  removeUser();
};

/**
 * Check if user has specific role
 */
export const hasRole = (role: string): boolean => {
  const user = getUser();
  return user?.role === role;
};

/**
 * Check if user is admin
 */
export const isAdmin = (): boolean => {
  const user = getUser();
  return user?.role === 'SuperAdmin' || user?.role === 'ProductAdmin' || user?.role === 'PointsAdmin';
};
