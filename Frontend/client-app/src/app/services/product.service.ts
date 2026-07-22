import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Product {
  id: string;
  productName: string;
  price: number;
  description: string;
  imageUrl: string;
  category: string;
  stockQuantity: number;
  reservedQuantity: number;
  availableQuantity: number;
  isAvailable: boolean;
}

export interface CreateProductRequest {
  productName: string;
  price: number;
  description: string;
  imageUrl: string;
  category?: string;
  stockQuantity: number;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/inventory/products`);
  }

  createProduct(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/inventory/products`, request);
  }

  restockProduct(productId: string, quantity: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/inventory/products/${productId}/restock`, { quantity });
  }

  deleteProduct(productId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/inventory/products/${productId}`);
  }
}
