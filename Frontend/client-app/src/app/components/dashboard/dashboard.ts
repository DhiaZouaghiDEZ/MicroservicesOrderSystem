import { Component, inject, signal, effect } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { interval } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { InventoryService } from '../../services/inventory.service';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
})
export class Dashboard {
  private inventoryService = inject(InventoryService);
  private orderService = inject(OrderService);

  // Poll every 3 seconds to show live Saga updates
  private refresh$ = interval(3000).pipe(switchMap(() => this.orderService.getOrders()));

  // Convert observables to signals (modern reactive pattern)
  inventory = toSignal(this.inventoryService.getInventory(), { initialValue: [] });
  orders = toSignal(this.refresh$, { initialValue: [] });

  // Computed signal - automatically recalculates when orders change
  completedOrders = signal(0);

  constructor() {
    effect(() => {
      const completed = this.orders().filter((o) => o.status === 'Completed').length;
      this.completedOrders.set(completed);
    });
  }
  getStatusClass(status: string): string {
    const classes: Record<string, string> = {
      Pending: 'bg-yellow-100 text-yellow-800',
      Processing: 'bg-blue-100 text-blue-800',
      Completed: 'bg-green-100 text-green-800',
      Failed: 'bg-red-100 text-red-800',
    };
    return classes[status] || 'bg-gray-100 text-gray-800';
  }
}
