import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { loadStripe, StripeCardElement } from '@stripe/stripe-js';
import { PaymentsService } from '../../../api-services/payments/payments.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
})
export class CheckoutComponent implements OnInit {
  bookingId!: number;
  clientSecret!: string;

  isLoading = false;
  cardError?: string;

  private stripe: any;
  private card!: StripeCardElement;

  private stripePublicKey =
    'pk_test_51SmMLOPNRubBTw6ncrcLkJsa4oCrLODXvVnHfwy2BxvrRG2ftkeaquWU8WBUmanMgih4fqH7byvbXqIyHnXIw5w400hNaftNVn'; // temporary

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

    const result = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: {
        card: this.card,
      },
    });

    if (result.error) {
      this.cardError = result.error.message ?? 'Payment failed';
    }
  }
}
