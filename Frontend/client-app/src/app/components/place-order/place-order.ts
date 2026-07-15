import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrderService, CreateOrderRequest } from '../../services/order.service';
import { InventoryService } from '../../services/inventory.service';

@Component({
  selector: 'app-place-order',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './place-order.html',
})
export class PlaceOrder {
  private fb = inject(FormBuilder);
  private orderService = inject(OrderService);
  private inventoryService = inject(InventoryService);

  inventory = signal<any[]>([]);
  isSubmitting = signal(false);
  orderResult = signal<{ success: boolean; orderId: string } | null>(null);

  orderForm = this.fb.group({
    productName: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]],
    amount: [0, [Validators.required, Validators.min(1)]],
  });

  ngOnInit() {
    this.inventoryService.getInventory().subscribe((data) => this.inventory.set(data));
  }

  selectProduct(item: any) {
    this.orderForm.patchValue({ productName: item.productName });
  }

  onSubmit() {
    if (this.orderForm.valid) {
      this.isSubmitting.set(true);
      this.orderResult.set(null);

      const request = this.orderForm.value as CreateOrderRequest;

      this.orderService.createOrder(request).subscribe({
        next: (response) => {
          this.isSubmitting.set(false);
          this.orderResult.set({ success: true, orderId: response.orderId });
          this.orderForm.reset({ quantity: 1, amount: 0 });
        },
        error: () => {
          this.isSubmitting.set(false);
          this.orderResult.set({ success: false, orderId: 'N/A' });
        },
      });
    }
  }
}
