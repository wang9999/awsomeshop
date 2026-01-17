import { get, post, put, del } from '../utils/request';

export interface Address {
  id: string;
  receiverName: string;
  receiverPhone: string;
  province?: string;
  city?: string;
  district?: string;
  detailAddress: string;
  isDefault: boolean;
}

export interface CreateAddressRequest {
  receiverName: string;
  receiverPhone: string;
  province?: string;
  city?: string;
  district?: string;
  detailAddress: string;
  isDefault: boolean;
}

export interface UpdateAddressRequest {
  receiverName?: string;
  receiverPhone?: string;
  province?: string;
  city?: string;
  district?: string;
  detailAddress?: string;
  isDefault?: boolean;
}

/**
 * Get all addresses
 */
export const getAddresses = (): Promise<Address[]> => {
  return get<Address[]>('/addresses');
};

/**
 * Get address by ID
 */
export const getAddressById = (id: string): Promise<Address> => {
  return get<Address>(`/addresses/${id}`);
};

/**
 * Create address
 */
export const createAddress = (data: CreateAddressRequest): Promise<Address> => {
  return post<Address>('/addresses', data);
};

/**
 * Update address
 */
export const updateAddress = (id: string, data: UpdateAddressRequest): Promise<Address> => {
  return put<Address>(`/addresses/${id}`, data);
};

/**
 * Delete address
 */
export const deleteAddress = (id: string): Promise<{ message: string }> => {
  return del<{ message: string }>(`/addresses/${id}`);
};

/**
 * Set address as default
 */
export const setDefaultAddress = (id: string): Promise<Address> => {
  return put<Address>(`/addresses/${id}/default`, {});
};
