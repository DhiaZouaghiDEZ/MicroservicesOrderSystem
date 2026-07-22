import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './checkout.html',
})
export class Checkout {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private orderService = inject(OrderService);

  productId = '';
  productName = '';
  price = 0;
  quantity = signal(1);
  isProcessing = signal(false);
  orderResult = signal<{ success: boolean; message: string } | null>(null);

  paymentForm = this.fb.group({
    cardNumber: ['', [Validators.required, Validators.pattern(/^\d{13,19}$/)]],
    cardHolder: ['', Validators.required],
    expiryDate: ['', [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]],
    cvv: ['', [Validators.required, Validators.pattern(/^\d{3,4}$/)]],
  });

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.productId = params['productId'];
      this.productName = params['productName'];
      this.price = parseFloat(params['price']);
    });
  }

  updateQuantity(value: number) {
    if (value >= 1) {
      this.quantity.set(value);
    }
  }

  get total(): number {
    return this.price * this.quantity();
  }

  onConfirmPayment() {
    if (this.paymentForm.valid) {
      this.isProcessing.set(true);
      this.orderResult.set(null);

      const orderRequest = {
        productId: this.productId,
        quantity: this.quantity(),
        cardNumber: this.paymentForm.value.cardNumber!,
      };

      this.orderService.createOrder(orderRequest).subscribe({
        next: (response) => {
          this.isProcessing.set(false);
          this.orderResult.set({
            success: true,
            message: `Payment successful! Order ID: ${response.orderId}. You will receive a confirmation SMS shortly.`,
          });
          setTimeout(() => this.router.navigate(['/order-history']), 3000);
        },
        error: (err) => {
          this.isProcessing.set(false);
          this.orderResult.set({
            success: false,
            message: err.error?.message || 'Payment failed. Please check your card details.',
          });
        },
      });
    }
  }
}
