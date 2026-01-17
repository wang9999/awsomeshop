// Form validation utility functions

/**
 * Validate email format
 */
export const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

/**
 * Validate phone number (Chinese format)
 */
export const validatePhone = (phone: string): boolean => {
  const phoneRegex = /^1[3-9]\d{9}$/;
  return phoneRegex.test(phone);
};

/**
 * Validate password strength
 * At least 6 characters
 */
export const validatePassword = (password: string): boolean => {
  return password.length >= 6;
};

/**
 * Validate required field
 */
export const validateRequired = (value: any): boolean => {
  if (typeof value === 'string') {
    return value.trim().length > 0;
  }
  return value !== null && value !== undefined;
};

/**
 * Validate number range
 */
export const validateNumberRange = (value: number, min?: number, max?: number): boolean => {
  if (min !== undefined && value < min) return false;
  if (max !== undefined && value > max) return false;
  return true;
};

/**
 * Validate positive integer
 */
export const validatePositiveInteger = (value: number): boolean => {
  return Number.isInteger(value) && value > 0;
};

/**
 * Validate non-negative integer
 */
export const validateNonNegativeInteger = (value: number): boolean => {
  return Number.isInteger(value) && value >= 0;
};
