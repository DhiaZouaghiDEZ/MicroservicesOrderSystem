import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { InventoryManagement } from './components/inventory-management/inventory-management';
import { PlaceOrder } from './components/place-order/place-order';
import { OrderHistory } from './components/order-history/order-history';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'inventory', component: InventoryManagement },
  { path: 'order', component: PlaceOrder },
  { path: 'history', component: OrderHistory },
  { path: '**', redirectTo: '/dashboard' },
];
