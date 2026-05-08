import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
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
  private payments = inject(PaymentsService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  readonly amountPresets = [100, 200, 500, 1000];
  readonly customMin = 1001;

  selectedPreset: number | null = null;
  isLoading = false;
  isReady = false;
  paymentSucceeded = false;

  cardError?: string;
  successMessage?: string;

  form = this.fb.group({
    customAmount: [null as number | null, [Validators.min(this.customMin)]],
    description: ['', [Validators.maxLength(500)]],
  });

  get customAmountCtrl() { return this.form.controls.customAmount; }

  private stripe: Stripe | null = null;
  private card!: StripeCardElement;

  get effectiveAmount(): number | null {
    const custom = this.form.controls.customAmount.value;
    return this.selectedPreset ?? (custom && custom > 0 ? custom : null);
  }

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

  selectPreset(value: number): void {
    this.selectedPreset = value;
    this.form.controls.customAmount.setValue(null);
    this.cardError = undefined;
  }

  onCustomAmountChange(): void {
    this.selectedPreset = null;
    this.cardError = undefined;
  }

  goToProfile() {
    this.router.navigate(['/client/profile']);
  }

  async pay() {
    this.cardError = undefined;
    this.successMessage = undefined;

    if (!this.stripe || !this.isReady) return;

    const amount = this.effectiveAmount;

    if (!amount || amount <= 0) {
      this.cardError = 'Please select a top-up amount or enter a custom amount above 1,000 BAM.';
      return;
    }

    if (this.selectedPreset === null && amount <= 1000) {
      this.cardError = 'Custom top-up amounts must exceed 1,000 BAM. Use the preset buttons for standard amounts.';
      return;
    }

    this.isLoading = true;

    const description = this.form.controls.description.value;

    this.payments
      .createWalletTopUpCardIntent({
        amount,
        description: description || null,
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
