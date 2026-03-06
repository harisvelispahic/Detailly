import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { loadStripe, Stripe, StripeCardElement } from '@stripe/stripe-js';
import { PaymentsService } from '../../../../api-services/payments/payments-api.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-booking-pay-card-page',
  templateUrl: './booking-pay-card-page.component.html',
  standalone: false,
})
export class BookingPayCardPageComponent implements OnInit, OnDestroy {
  bookingId!: number;

  isLoading = false;
  isReady = false;
  cardError?: string;

  private stripe: Stripe | null = null;
  private card?: StripeCardElement;
  private clientSecret?: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private payments: PaymentsService,
  ) {}

  async ngOnInit(): Promise<void> {
    this.bookingId = Number(this.route.snapshot.paramMap.get('bookingId'));

    if (!this.bookingId || Number.isNaN(this.bookingId)) {
      this.cardError = 'Invalid booking id.';
      return;
    }

    this.isLoading = true;
    this.cardError = undefined;

    try {
      // 1) Initialize Stripe
      this.stripe = await loadStripe(environment.stripePublishableKey);
      if (!this.stripe) {
        this.cardError = 'Stripe failed to initialize.';
        this.isLoading = false;
        return;
      }

      // 2) Ask backend for clientSecret
      this.payments.createCardIntent(this.bookingId).subscribe({
        next: (res) => {
          this.clientSecret = res.clientSecret;

          // 3) Mount card element
          const elements = this.stripe!.elements();
          this.card = elements.create('card');
          this.card.mount('#card-element');

          this.isReady = true;
          this.isLoading = false;
        },
        error: (err) => {
          this.cardError = err?.error?.message ?? 'Failed to create payment intent.';
          this.isLoading = false;
        },
      });
    } catch {
      this.cardError = 'Stripe initialization failed.';
      this.isLoading = false;
    }
  }

  ngOnDestroy(): void {
    try {
      this.card?.unmount();
    } catch {
      // ignore
    }
  }

  async pay(): Promise<void> {
    if (!this.stripe || !this.card || !this.clientSecret) return;

    this.isLoading = true;
    this.cardError = undefined;

    const result = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: { card: this.card },
    });

    this.isLoading = false;

    if (result.error) {
      const msg = result.error.message ?? 'Payment failed';
      this.cardError = msg;
      alert(`❌ Payment failed: ${msg}`);
      return;
    }

    const status = result.paymentIntent?.status;
    if (status === 'succeeded') {
      alert('✅ Payment successful! Booking will confirm after webhook processing.');
      // Optional: navigate back to booking details
      this.router.navigate(['/client/bookings', this.bookingId]);
      return;
    }

    alert(`ℹ️ Payment submitted (status: ${status ?? 'unknown'})`);
  }

  back(): void {
    this.router.navigate(['/client/bookings', this.bookingId]);
  }
}
