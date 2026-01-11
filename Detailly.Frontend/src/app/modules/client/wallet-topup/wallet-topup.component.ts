import { Component, OnInit } from '@angular/core';
import { loadStripe, Stripe, StripeCardElement } from '@stripe/stripe-js';
import { PaymentsService } from '../../../api-services/payments/payments.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-wallet-topup',
  templateUrl: './wallet-topup.component.html',
  standalone: false,
})
export class WalletTopUpComponent implements OnInit {
  amount: number = 0;
  description: string = '';

  isLoading = false;
  isReady = false;

  cardError?: string;
  successMessage?: string;

  private stripe: Stripe | null = null;
  private card!: StripeCardElement;

  constructor(private payments: PaymentsService) {}

  async ngOnInit() {
    this.stripe = await loadStripe(environment.stripePublishableKey);

    if (!this.stripe) {
      this.cardError = 'Stripe failed to initialize.';
      return;
    }

    const elements = this.stripe.elements();
    this.card = elements.create('card');
    this.card.mount('#card-element');

    this.isReady = true;
  }

  async pay() {
    this.cardError = undefined;
    this.successMessage = undefined;

    if (!this.stripe || !this.isReady) return;

    if (!this.amount || this.amount <= 0) {
      this.cardError = 'Enter a valid amount.';
      alert('❌ Enter a valid amount.');
      return;
    }

    this.isLoading = true;

    // Ask backend to create wallet top-up payment intent
    this.payments
      .createWalletTopUpCardIntent({
        amount: this.amount,
        description: this.description || null,
      })
      .subscribe({
        next: async (res) => {
          const clientSecret = res.clientSecret;

          const result = await this.stripe!.confirmCardPayment(clientSecret, {
            payment_method: {
              card: this.card,
            },
          });

          this.isLoading = false;

          if (result.error) {
            const msg = result.error.message ?? 'Payment failed.';
            this.cardError = msg;
            alert(`❌ Wallet top up failed: ${msg}`);
            return;
          }

          // Stripe can return statuses like succeeded / processing / requires_action
          const status = result.paymentIntent?.status;

          if (status === 'succeeded') {
            this.successMessage =
              'Payment successful ✅ Wallet will update after webhook confirmation.';
            alert('✅ Wallet top up successful!');
          } else {
            this.successMessage =
              'Payment submitted ✅ Wallet will update after webhook confirmation.';
            alert(`ℹ️ Payment submitted (status: ${status ?? 'unknown'})`);
          }

          // reset input
          this.amount = 0;
          this.description = '';
        },
        error: () => {
          this.isLoading = false;
          this.cardError = 'Failed to create payment intent.';
          alert('❌ Failed to create payment intent.');
        },
      });
  }
}
