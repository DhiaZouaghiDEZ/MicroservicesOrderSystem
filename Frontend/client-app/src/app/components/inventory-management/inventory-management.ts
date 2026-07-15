import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InventoryService, AddStockRequest } from '../../services/inventory.service';

@Component({
  selector: 'app-inventory-management',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './inventory-management.html',
})
export class InventoryManagement {
  private fb = inject(FormBuilder);
  private inventoryService = inject(InventoryService);

  inventory = signal<any[]>([]);
  successMessage = signal('');

  addStockForm = this.fb.group({
    productName: ['', Validators.required],
    quantity: [0, [Validators.required, Validators.min(1)]],
  });

  ngOnInit() {
    this.loadInventory();
  }

  loadInventory() {
    this.inventoryService.getInventory().subscribe((data) => this.inventory.set(data));
  }

  onAddStock() {
    if (this.addStockForm.valid) {
      const request = this.addStockForm.value as AddStockRequest;

      this.inventoryService.addStock(request).subscribe(() => {
        this.successMessage.set('Stock added successfully!');
        this.addStockForm.reset();
        this.loadInventory();
        setTimeout(() => this.successMessage.set(''), 3000);
      });
    }
  }

  getStatusClass(quantity: number): string {
    if (quantity > 10) return 'bg-green-100 text-green-800';
    if (quantity > 0) return 'bg-yellow-100 text-yellow-800';
    return 'bg-red-100 text-red-800';
  }

  getStatusText(quantity: number): string {
    if (quantity > 10) return 'In Stock';
    if (quantity > 0) return 'Low Stock';
    return 'Out of Stock';
  }
}
