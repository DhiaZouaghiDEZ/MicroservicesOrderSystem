import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService, Product, CreateProductRequest } from '../../services/product.service';

@Component({
  selector: 'app-product-management',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './product-management.html',
})
export class ProductManagement {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);

  products = signal<Product[]>([]);
  successMessage = signal('');
  errorMessage = signal('');

  productForm = this.fb.group({
    productName: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0.01)]],
    category: ['General', Validators.required], // 🔥 Added
    description: ['', Validators.required],
    imageUrl: ['', [Validators.required]],
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
      this.productService.deleteProduct(productId).subscribe(() => {
        this.successMessage.set('Product deleted successfully!');
        this.loadProducts();
        setTimeout(() => this.successMessage.set(''), 3000);
      });
    }
  }
}
