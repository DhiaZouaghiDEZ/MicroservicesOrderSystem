import { Component, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService, Product, CreateProductRequest } from '../../services/product.service';

@Component({
  selector: 'app-product-management',
  standalone: true,
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './product-management.html',
})
export class ProductManagement {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);

  products = signal<Product[]>([]);
  successMessage = signal('');
  errorMessage = signal('');
  restockProductId = signal<string | null>(null);
  restockQuantity = signal(0);

  productForm = this.fb.group({
    productName: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0.01)]],
    category: ['General', Validators.required],
    description: ['', Validators.required],
    imageUrl: [
      'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400',
      [Validators.required],
    ],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
  });

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getProducts().subscribe((data) => this.products.set(data));
  }

  onCreateProduct() {
    if (this.productForm.valid) {
      const request = this.productForm.value as CreateProductRequest;
      this.productService.createProduct(request).subscribe({
        next: () => {
          this.successMessage.set('Product created successfully!');
          this.errorMessage.set('');
          this.productForm.reset({
            imageUrl: 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400',
            stockQuantity: 0,
            category: 'General',
          });
          this.loadProducts();
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: (err) => {
          if (err.status === 409) {
            this.errorMessage.set(err.error.message || 'A product with this name already exists.');
          } else {
            this.errorMessage.set('Failed to create product. Please try again.');
          }
          this.successMessage.set('');
          setTimeout(() => this.errorMessage.set(''), 5000);
        },
      });
    }
  }

  onDeleteProduct(productId: string) {
    if (confirm('Are you sure you want to delete this product? This cannot be undone.')) {
      this.productService.deleteProduct(productId).subscribe({
        next: () => {
          this.successMessage.set('Product deleted successfully!');
          this.errorMessage.set('');
          this.loadProducts();
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: () => {
          this.errorMessage.set('Failed to delete product. Please try again.');
          this.successMessage.set('');
          setTimeout(() => this.errorMessage.set(''), 5000);
        },
      });
    }
  }

  showRestockModal(productId: string) {
    this.restockProductId.set(productId);
    this.restockQuantity.set(10);
  }

  hideRestockModal() {
    this.restockProductId.set(null);
    this.restockQuantity.set(0);
  }

  onRestock() {
    const productId = this.restockProductId();
    const quantity = this.restockQuantity();

    if (productId && quantity > 0) {
      this.productService.restockProduct(productId, quantity).subscribe({
        next: () => {
          this.successMessage.set(`Successfully restocked ${quantity} units!`);
          this.errorMessage.set('');
          this.hideRestockModal();
          this.loadProducts();
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: () => {
          this.errorMessage.set('Failed to restock product. Please try again.');
          this.successMessage.set('');
          setTimeout(() => this.errorMessage.set(''), 5000);
        },
      });
    }
  }

  updateRestockQuantity(value: number) {
    if (value >= 1) {
      this.restockQuantity.set(value);
    }
  }
}
