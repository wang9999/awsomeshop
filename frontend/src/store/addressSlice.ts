import { create } from 'zustand';
import { Address } from '../services/addressService';

interface AddressState {
  addresses: Address[];
  defaultAddress: Address | null;
  loading: boolean;
  
  // Actions
  setAddresses: (addresses: Address[]) => void;
  setDefaultAddress: (address: Address | null) => void;
  setLoading: (loading: boolean) => void;
  addAddress: (address: Address) => void;
  updateAddress: (id: string, address: Address) => void;
  removeAddress: (id: string) => void;
}

export const useAddressStore = create<AddressState>((set) => ({
  addresses: [],
  defaultAddress: null,
  loading: false,

  setAddresses: (addresses: Address[]) => {
    const defaultAddr = addresses.find(addr => addr.isDefault) || null;
    set({ addresses, defaultAddress: defaultAddr });
  },

  setDefaultAddress: (address: Address | null) => {
    set({ defaultAddress: address });
  },

  setLoading: (loading: boolean) => {
    set({ loading });
  },

  addAddress: (address: Address) => {
    set((state) => ({
      addresses: [...state.addresses, address],
      defaultAddress: address.isDefault ? address : state.defaultAddress,
    }));
  },

  updateAddress: (id: string, address: Address) => {
    set((state) => ({
      addresses: state.addresses.map(addr => addr.id === id ? address : addr),
      defaultAddress: address.isDefault ? address : state.defaultAddress,
    }));
  },

  removeAddress: (id: string) => {
    set((state) => ({
      addresses: state.addresses.filter(addr => addr.id !== id),
      defaultAddress: state.defaultAddress?.id === id ? null : state.defaultAddress,
    }));
  },
}));
