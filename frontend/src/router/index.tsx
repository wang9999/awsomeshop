import { createBrowserRouter, Navigate } from 'react-router-dom';
import { isAuthenticated, isAdmin } from '../utils/auth';

// Layouts
import MainLayout from '../components/Layout/MainLayout';
import AdminLayout from '../components/Layout/AdminLayout';

// Employee pages
import EmployeeLogin from '../pages/employee/Login/Login';
import ProductList from '../pages/employee/ProductList/ProductList';
import ProductDetail from '../pages/employee/ProductDetail/ProductDetail';
import Cart from '../pages/employee/Cart/Cart';
import Points from '../pages/employee/Points/Points';
import Orders from '../pages/employee/Orders/Orders';
import OrderDetail from '../pages/employee/OrderDetail/OrderDetail';
import Checkout from '../pages/employee/Checkout/Checkout';
import Addresses from '../pages/employee/Addresses/Addresses';

// Admin pages
import AdminLogin from '../pages/admin/Login/Login';
import Dashboard from '../pages/admin/Dashboard/Dashboard';
import PointsManagement from '../pages/admin/PointsManagement/PointsManagement';
import ProductManagement from '../pages/admin/ProductManagement/ProductManagement';
import OrderManagement from '../pages/admin/OrderManagement/OrderManagement';
import AuditLogs from '../pages/admin/AuditLogs/AuditLogs';

// Protected route wrapper for employee pages
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }
  return <>{children}</>;
};

// Protected route wrapper for admin pages
const AdminRoute = ({ children }: { children: React.ReactNode }) => {
  if (!isAuthenticated()) {
    return <Navigate to="/admin/login" replace />;
  }
  if (!isAdmin()) {
    return <Navigate to="/" replace />;
  }
  return <>{children}</>;
};

const router = createBrowserRouter([
  // Employee routes
  {
    path: '/login',
    element: <EmployeeLogin />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <ProductList />,
      },
      {
        path: 'products',
        element: <ProductList />,
      },
      {
        path: 'products/:id',
        element: <ProductDetail />,
      },
      {
        path: 'cart',
        element: <Cart />,
      },
      {
        path: 'points',
        element: <Points />,
      },
      {
        path: 'orders',
        element: <Orders />,
      },
      {
        path: 'orders/:id',
        element: <OrderDetail />,
      },
      {
        path: 'checkout',
        element: <Checkout />,
      },
      {
        path: 'addresses',
        element: <Addresses />,
      },
    ],
  },
  
  // Admin routes
  {
    path: '/admin/login',
    element: <AdminLogin />,
  },
  {
    path: '/admin',
    element: (
      <AdminRoute>
        <AdminLayout />
      </AdminRoute>
    ),
    children: [
      {
        index: true,
        element: <Dashboard />,
      },
      {
        path: 'products',
        element: <ProductManagement />,
      },
      {
        path: 'points',
        element: <PointsManagement />,
      },
      {
        path: 'orders',
        element: <OrderManagement />,
      },
      {
        path: 'audit-logs',
        element: <AuditLogs />,
      },
    ],
  },
  
  // Catch all - redirect to home
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);

export default router;
