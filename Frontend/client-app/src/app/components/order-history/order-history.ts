import { Component, inject, signal } from '@angular/core';
import { DatePipe, SlicePipe } from '@angular/common';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [DatePipe, SlicePipe],
  templateUrl: './order-history.html',
})
export class OrderHistory {
  private orderService = inject(OrderService);

  orders = signal<any[]>([]);

  ngOnInit() {
    this.orderService.getOrders().subscribe((data) => this.orders.set(data));
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
