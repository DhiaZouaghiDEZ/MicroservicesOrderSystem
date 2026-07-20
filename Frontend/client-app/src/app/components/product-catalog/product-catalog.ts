import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService, Product } from '../../services/product.service';

@Component({
  selector: 'app-product-catalog',
  standalone: true,
  templateUrl: './product-catalog.html',
})
export class ProductCatalog {
  private productService = inject(ProductService);
  private router = inject(Router);

  products = signal<Product[]>([]);

  ngOnInit() {
    this.productService.getProducts().subscribe((data) => this.products.set(data));
  }

  buyNow(product: Product) {
    if (!product.isAvailable) return;

    this.router.navigate(['/checkout'], {
      queryParams: {
        productId: product.id,
        productName: product.productName,
        price: product.price,
      },
    });
  }
}
