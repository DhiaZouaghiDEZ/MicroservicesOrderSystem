import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Order {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  amount: number;
  status: string;
  createdAt: string;
}

export interface CreateOrderRequest {
  productId: string;
  quantity: number;
  cardNumber: string;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  createOrder(request: CreateOrderRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/orders`, request);
  }

  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/orders`);
  }
}
