import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { loadStripe, StripeCardElement } from '@stripe/stripe-js';
import { PaymentsService } from '../../../api-services/payments/payments.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  standalone: false,
})
export class CheckoutComponent implements OnInit {
  bookingId!: number;
  clientSecret!: string;

  isLoading = false;
  cardError?: string;

  private stripe: any;
  private card!: StripeCardElement;

  private stripePublicKey = environment.stripePublishableKey;

  constructor(private route: ActivatedRoute, private payments: PaymentsService) {}

  async ngOnInit() {
    this.bookingId = Number(this.route.snapshot.paramMap.get('bookingId'));

    this.isLoading = true;

    this.payments.createCardIntent(this.bookingId).subscribe({
      next: async (res) => {
        this.clientSecret = res.clientSecret;

        this.stripe = await loadStripe(this.stripePublicKey);

        const elements = this.stripe!.elements();

        this.card = elements.create('card');

        this.card.mount('#card-element');

        this.isLoading = false;
      },
      error: () => {
        this.cardError = 'Failed to create payment intent';
        this.isLoading = false;
      },
    });
  }

  async pay() {
    if (!this.stripe || !this.clientSecret) return;

    this.isLoading = true;
    this.cardError = undefined;

    const result = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: {
        card: this.card,
      },
    });

    this.isLoading = false;

    if (result.error) {
      const msg = result.error.message ?? 'Payment failed';
      this.cardError = msg;
      alert(`❌ Payment failed: ${msg}`);
      return;
    }

    // If no error, Stripe returned a PaymentIntent
    if (result.paymentIntent?.status === 'succeeded') {
      alert('✅ Payment successful! Thank you.');
    } else {
      alert(`ℹ️ Payment status: ${result.paymentIntent?.status ?? 'unknown'}`);
    }
  }
}
