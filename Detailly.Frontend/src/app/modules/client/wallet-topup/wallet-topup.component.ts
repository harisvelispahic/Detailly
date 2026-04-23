import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { loadStripe, Stripe, StripeCardElement } from '@stripe/stripe-js';
import { PaymentsService } from '../../../api-services/payments/payments-api.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-wallet-topup',
  templateUrl: './wallet-topup.component.html',
  styleUrl: './wallet-topup.component.scss',
  standalone: false,
})
export class WalletTopUpComponent implements OnInit {
  amount: number | null = null;
  description: string = '';
  readonly amountPresets = [10, 25, 50, 100];

  isLoading = false;
  isReady = false;
  paymentSucceeded = false;

  cardError?: string;
  successMessage?: string;

  private stripe: Stripe | null = null;
  private card!: StripeCardElement;

  constructor(
    private payments: PaymentsService,
    private router: Router,
  ) {}

  async ngOnInit() {
    this.stripe = await loadStripe(environment.stripePublishableKey);

    if (!this.stripe) {
      this.cardError = 'Stripe failed to initialize.';
      return;
    }

    const elements = this.stripe.elements();
    this.card = elements.create('card', {
      style: {
        base: {
          color: '#f2f2f5',
          fontFamily: "'Inter', sans-serif",
          fontSize: '15px',
          fontSmoothing: 'antialiased',
          '::placeholder': { color: '#6b6b82' },
          iconColor: '#f2f2f5',
        },
        invalid: {
          color: '#ef4444',
          iconColor: '#ef4444',
        },
      },
    });
    this.card.mount('#card-element');

    this.isReady = true;
  }

  setAmount(value: number) {
    this.amount = value;
  }

  goToProfile() {
    this.router.navigate(['/client/profile']);
  }

  async pay() {
    this.cardError = undefined;
    this.successMessage = undefined;

    if (!this.stripe || !this.isReady) return;

    if (!this.amount || this.amount <= 0) {
      this.cardError = 'Please enter a valid amount.';
      return;
    }

    this.isLoading = true;

    this.payments
      .createWalletTopUpCardIntent({
        amount: this.amount,
        description: this.description || null,
      })
      .subscribe({
        next: async (res) => {
          const result = await this.stripe!.confirmCardPayment(res.clientSecret, {
            payment_method: { card: this.card },
          });

          this.isLoading = false;

          if (result.error) {
            this.cardError = result.error.message ?? 'Payment failed.';
            return;
          }

          const status = result.paymentIntent?.status;

          if (status === 'succeeded') {
            this.successMessage = 'Payment successful! Your wallet balance will update shortly.';
          } else {
            this.successMessage = `Payment submitted (status: ${status ?? 'unknown'}). Your wallet will update after confirmation.`;
          }

          this.paymentSucceeded = true;
        },
        error: () => {
          this.isLoading = false;
          this.cardError = 'Failed to create payment intent. Please try again.';
        },
      });
  }
}
