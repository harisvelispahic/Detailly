import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { loadStripe, StripeCardElement } from '@stripe/stripe-js';
import { PaymentsService } from '../../../../api-services/payments/payments-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-order-payment',
  templateUrl: './order-payment.component.html',
  standalone: false,
})
export class OrderPaymentComponent implements OnInit {
  orderId!: number;
  clientSecret!: string;

  isLoading = false;
  cardError?: string;

  private stripe: any;
  private card!: StripeCardElement;

  private stripePublicKey = environment.stripePublishableKey;

  constructor(
    private route: ActivatedRoute,
    private payments: PaymentsService,
    private toaster: ToasterService,
  ) {}

  async ngOnInit() {
    this.orderId = Number(this.route.snapshot.paramMap.get('orderId'));

    this.isLoading = true;

    this.payments.createOrderCardIntent(this.orderId).subscribe({
      next: async (res) => {
        this.clientSecret = res.clientSecret;

        this.stripe = await loadStripe(this.stripePublicKey);
        const elements = this.stripe!.elements();

        this.card = elements.create('card');
        this.card.mount('#card-element');

        this.isLoading = false;
      },
      error: () => {
        this.cardError = 'Failed to create order payment intent';
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
      this.toaster.error(msg);
      return;
    }

    if (result.paymentIntent?.status === 'succeeded') {
      this.toaster.success('Order payment successful! Thank you.');
    } else {
      this.toaster.info(`Payment status: ${result.paymentIntent?.status ?? 'unknown'}`);
    }
  }
}
