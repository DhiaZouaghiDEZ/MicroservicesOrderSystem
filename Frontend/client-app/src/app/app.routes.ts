import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { ProductCatalog } from './components/product-catalog/product-catalog';
import { ProductManagement } from './components/product-management/product-management';
import { Checkout } from './components/checkout/checkout';
import { OrderHistory } from './components/order-history/order-history';

export const routes: Routes = [
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'products', component: ProductCatalog },
  { path: 'admin/products', component: ProductManagement },
  { path: 'checkout', component: Checkout },
  { path: 'order-history', component: OrderHistory },
  { path: '**', redirectTo: '/products' },
];
