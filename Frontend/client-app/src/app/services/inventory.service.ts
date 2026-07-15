import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface InventoryItem {
  id: string;
  productName: string;
  stockQuantity: number;
}

export interface AddStockRequest {
  productName: string;
  quantity: number;
}

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getInventory(): Observable<InventoryItem[]> {
    return this.http.get<InventoryItem[]>(`${this.apiUrl}/inventory`);
  }

  addStock(request: AddStockRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/inventory/add`, request);
  }
}
