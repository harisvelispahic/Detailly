import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { loadStripe, Stripe, StripeCardElement } from '@stripe/stripe-js';
import { BookingsService } from '../../../../api-services/bookings/bookings-api.service';
import { PaymentsService } from '../../../../api-services/payments/payments-api.service';
import { GetBookingByIdQueryDto } from '../../../../api-services/bookings/bookings-api.models';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton, DialogType } from '../../../shared/models/dialog-config.model';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-booking-payment-page',
  templateUrl: './booking-payment-page.component.html',
  styleUrl: './booking-payment-page.component.scss',
  standalone: false,
})
export class BookingPaymentPageComponent implements OnInit, OnDestroy {
  id!: number;

  isLoadingBooking = false;
  isLoadingWallet = false;
  isPayingWallet = false;
  isInitializingCard = false;
  isPayingCard = false;

  booking?: GetBookingByIdQueryDto;
  walletBalance?: number;
  walletCurrency = 'BAM';

  error?: string;
  cardError?: string;

  selectedMethod: 'wallet' | 'card' | null = null;
  isCardReady = false;

  private stripe: Stripe | null = null;
  private card?: StripeCardElement;
  private clientSecret?: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingsService: BookingsService,
    private paymentsService: PaymentsService,
    private dialogHelper: DialogHelperService,
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadBooking();
    this.loadWallet();
  }

  ngOnDestroy(): void {
    try {
      this.card?.unmount();
    } catch {
      // ignore
    }
  }

  loadBooking(): void {
    this.isLoadingBooking = true;
    this.bookingsService.getById(this.id).subscribe({
      next: (res) => {
        this.booking = res;
        this.isLoadingBooking = false;
      },
      error: () => {
        this.error = 'Failed to load booking details.';
        this.isLoadingBooking = false;
      },
    });
  }

  loadWallet(): void {
    this.isLoadingWallet = true;
    this.paymentsService.getMyWallet().subscribe({
      next: (wallet) => {
        this.walletBalance = wallet.balance;
        this.walletCurrency = wallet.currency;
        this.isLoadingWallet = false;
      },
      error: () => {
        this.isLoadingWallet = false;
      },
    });
  }

  selectMethod(method: 'wallet' | 'card'): void {
    this.selectedMethod = method;
    if (method === 'card' && !this.isCardReady && !this.isInitializingCard) {
      setTimeout(() => this.initializeCard());
    }
  }

  async initializeCard(): Promise<void> {
    this.isInitializingCard = true;
    this.cardError = undefined;

    try {
      this.stripe = await loadStripe(environment.stripePublishableKey);
      if (!this.stripe) {
        this.cardError = 'Stripe failed to initialize.';
        this.isInitializingCard = false;
        return;
      }

      this.paymentsService.createCardIntent(this.id).subscribe({
        next: (res) => {
          this.clientSecret = res.clientSecret;
          const elements = this.stripe!.elements();
          this.card = elements.create('card', {
            style: {
              base: {
                color: '#fafafa',
                fontFamily: 'Inter, sans-serif',
                fontSize: '15px',
                '::placeholder': { color: '#888899' },
              },
              invalid: { color: '#e05252' },
            },
          });
          this.card.mount('#stripe-card-element');
          this.card.on('change', (event) => {
            this.cardError = event.error?.message;
          });
          this.isCardReady = true;
          this.isInitializingCard = false;
        },
        error: (err) => {
          this.cardError = err?.error?.message ?? 'Failed to create payment intent.';
          this.isInitializingCard = false;
        },
      });
    } catch {
      this.cardError = 'Stripe initialization failed.';
      this.isInitializingCard = false;
    }
  }

  async payWithCard(): Promise<void> {
    if (!this.stripe || !this.card || !this.clientSecret) return;

    this.isPayingCard = true;
    this.cardError = undefined;

    const result = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: { card: this.card },
    });

    this.isPayingCard = false;

    if (result.error) {
      this.cardError = result.error.message ?? 'Payment failed.';
      return;
    }

    this.showSuccessDialog('Your booking will be confirmed shortly after payment processing.', true);
  }

  payWithWallet(): void {
    this.isPayingWallet = true;
    this.error = undefined;

    this.paymentsService.payBookingWithWallet(this.id).subscribe({
      next: () => {
        this.isPayingWallet = false;
        this.showSuccessDialog('Your booking is now confirmed.');
      },
      error: (err) => {
        this.isPayingWallet = false;
        this.error = err?.error?.message ?? 'Failed to pay with wallet.';
      },
    });
  }

  private showSuccessDialog(message: string, awaitConfirmation = false): void {
    this.dialogHelper
      .open({
        type: DialogType.SUCCESS,
        title: 'Payment Successful',
        message,
        icon: 'check_circle',
        buttons: [{ type: DialogButton.OK }],
      })
      .subscribe(() => {
        this.router.navigate(
          ['/client/bookings', this.id],
          awaitConfirmation ? { queryParams: { awaitConfirmation: 'true' } } : {},
        );
      });
  }

  back(): void {
    this.router.navigate(['/client/bookings', this.id]);
  }

  topUpWallet(): void {
    this.router.navigate(['/client/wallet/top-up']);
  }

  get hasSufficientBalance(): boolean {
    if (this.walletBalance == null || !this.booking) return false;
    return this.walletBalance >= this.booking.totalPrice;
  }

  get isLoading(): boolean {
    return this.isLoadingBooking || this.isLoadingWallet;
  }

}
