import { Component, inject, signal, computed } from '@angular/core';
import { SlicePipe, DatePipe, DecimalPipe } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { interval } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { InventoryService } from '../../services/inventory.service';
import { OrderService, Order } from '../../services/order.service';
import { ProductService, Product } from '../../services/product.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
  imports: [SlicePipe, DatePipe, DecimalPipe],
})
export class Dashboard {
  private inventoryService = inject(InventoryService);
  private orderService = inject(OrderService);
  private productService = inject(ProductService);

  private refresh$ = interval(3000).pipe(switchMap(() => this.orderService.getOrders()));

  products = toSignal(this.productService.getProducts(), { initialValue: [] as Product[] });
  orders = toSignal(this.refresh$, { initialValue: [] as Order[] });

  productMap = computed(() => {
    const map = new Map<string, Product>();
    this.products().forEach((p) => map.set(p.id, p));
    return map;
  });

  totalProducts = computed(() => this.products().length);
  totalOrders = computed(() => this.orders().length);
  completedOrders = computed(() => this.orders().filter((o) => o.status === 'Completed').length);
  totalRevenue = computed(() =>
    this.orders()
      .filter((o) => o.status === 'Completed')
      .reduce((sum, o) => sum + o.amount, 0),
  );

  getProductName(productId: string): string {
    const product = this.productMap().get(productId);
    return product?.productName || 'Unknown Product';
  }

  getProductImage(productId: string): string {
    const product = this.productMap().get(productId);
    return product?.imageUrl || 'https://via.placeholder.com/150';
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
