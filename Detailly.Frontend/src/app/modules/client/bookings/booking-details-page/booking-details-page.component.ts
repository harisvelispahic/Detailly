import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingsService } from '../../../../api-services/bookings/bookings-api.service';
import {
  BookingStatus,
  GetBookingByIdQueryDto,
  PaymentTransactionStatus,
  ServiceMode,
} from '../../../../api-services/bookings/bookings-api.models';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton, DialogType } from '../../../shared/models/dialog-config.model';

@Component({
  selector: 'app-booking-details-page',
  templateUrl: './booking-details-page.component.html',
  styleUrl: './booking-details-page.component.scss',
  standalone: false,
})
export class BookingDetailsPageComponent implements OnInit, OnDestroy {
  id!: number;

  isLoading = false;
  isCancelling = false;

  error?: string;
  booking?: GetBookingByIdQueryDto;

  cancelReason = '';

  secondsRemaining: number | null = null;
  isAwaitingConfirmation = false;

  private countdownInterval?: ReturnType<typeof setInterval>;
  private pollInterval?: ReturnType<typeof setInterval>;
  private pollTimeoutHandle?: ReturnType<typeof setTimeout>;

  BookingStatus = BookingStatus;
  ServiceMode = ServiceMode;
  PaymentTransactionStatus = PaymentTransactionStatus;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingsService: BookingsService,
    private dialogHelper: DialogHelperService,
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    const awaitConfirmation = this.route.snapshot.queryParamMap.get('awaitConfirmation') === 'true';
    if (awaitConfirmation) {
      this.router.navigate([], { relativeTo: this.route, queryParams: {}, replaceUrl: true });
    }
    this.load(awaitConfirmation);
  }

  ngOnDestroy(): void {
    clearInterval(this.countdownInterval);
    this.stopPolling();
  }

  load(awaitConfirmation = false): void {
    this.isLoading = true;
    this.error = undefined;

    this.bookingsService.getById(this.id).subscribe({
      next: (res) => {
        this.booking = res;
        this.isLoading = false;
        this.startCountdown();
        if (awaitConfirmation && res.status === BookingStatus.PendingPayment) {
          this.startConfirmationPolling();
        }
      },
      error: () => {
        this.error = 'Failed to load booking details.';
        this.isLoading = false;
      },
    });
  }

  private startConfirmationPolling(): void {
    this.isAwaitingConfirmation = true;

    this.pollInterval = setInterval(() => {
      this.bookingsService.getById(this.id).subscribe({
        next: (res) => {
          if (res.status !== BookingStatus.PendingPayment) {
            this.stopPolling();
            this.booking = res;
            this.startCountdown();
          }
        },
      });
    }, 2000);

    this.pollTimeoutHandle = setTimeout(() => {
      this.stopPolling();
      this.bookingsService.getById(this.id).subscribe({
        next: (res) => {
          this.booking = res;
          this.startCountdown();
        },
      });
    }, 15000);
  }

  private stopPolling(): void {
    clearInterval(this.pollInterval);
    clearTimeout(this.pollTimeoutHandle);
    this.isAwaitingConfirmation = false;
  }

  private startCountdown(): void {
    clearInterval(this.countdownInterval);
    this.secondsRemaining = null;

    if (
      this.booking?.status !== BookingStatus.PendingPayment ||
      !this.booking.reservationExpiresAtUtc
    )
      return;

    const update = () => {
      const diff = Math.floor(
        (this.parseUtc(this.booking!.reservationExpiresAtUtc!).getTime() - Date.now()) / 1000,
      );
      this.secondsRemaining = Math.max(0, diff);
      if (this.secondsRemaining === 0) clearInterval(this.countdownInterval);
    };

    update();
    this.countdownInterval = setInterval(update, 1000);
  }

  get countdownDisplay(): string {
    if (this.secondsRemaining == null) return '';
    const m = Math.floor(this.secondsRemaining / 60);
    const s = this.secondsRemaining % 60;
    return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
  }

  cancel(): void {
    const message =
      this.booking?.status === BookingStatus.Confirmed
        ? `${this.refundNote}. This action cannot be undone.`
        : 'This will release your reservation. This action cannot be undone.';

    this.dialogHelper
      .open({
        type: DialogType.WARNING,
        title: 'Cancel Booking',
        message,
        icon: 'cancel',
        buttons: [
          { type: DialogButton.CANCEL },
          { type: DialogButton.YES, label: 'Yes, Cancel Booking', color: 'warn' },
        ],
      })
      .subscribe((result) => {
        if (result?.button !== DialogButton.YES) return;

        this.isCancelling = true;
        this.error = undefined;

        this.bookingsService
          .cancelBooking(this.id, { reason: this.cancelReason?.trim() || null })
          .subscribe({
            next: () => {
              this.isCancelling = false;
              this.load();
            },
            error: (err) => {
              this.isCancelling = false;
              this.error = err?.error?.message ?? 'Failed to cancel booking.';
            },
          });
      });
  }

  back(): void {
    this.router.navigate(['/client/bookings']);
  }

  get canCancel(): boolean {
    const s = this.booking?.status;
    return s === BookingStatus.Confirmed || s === BookingStatus.PendingPayment;
  }

  get canPay(): boolean {
    return this.booking?.status === BookingStatus.PendingPayment;
  }

  get durationMinutes(): number {
    if (!this.booking) return 0;
    const start = this.parseUtc(this.booking.startUtc);
    const end = this.parseUtc(this.booking.endUtc);
    return Math.round((end.getTime() - start.getTime()) / 60000);
  }

  get refundNote(): string {
    if (!this.booking) return '';
    const hoursUntilStart = (this.parseUtc(this.booking.startUtc).getTime() - Date.now()) / 3_600_000;
    if (hoursUntilStart >= 48) return '100% refund — more than 48 hours before start';
    if (hoursUntilStart >= 24) return '50% refund — between 24 and 48 hours before start';
    if (hoursUntilStart > 0) return '25% refund — less than 24 hours before start';
    return 'No refund — appointment has already started or passed';
  }

  getStatusLabel(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Draft:
        return 'Draft';
      case BookingStatus.PendingPayment:
        return 'Pending Payment';
      case BookingStatus.Confirmed:
        return 'Confirmed';
      case BookingStatus.Cancelled:
        return 'Cancelled';
      case BookingStatus.Completed:
        return 'Completed';
      case BookingStatus.Expired:
        return 'Expired';
      default:
        return 'Unknown';
    }
  }

  getStatusClass(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Confirmed:
        return 'status--confirmed';
      case BookingStatus.PendingPayment:
        return 'status--pending';
      case BookingStatus.Completed:
        return 'status--completed';
      case BookingStatus.Cancelled:
        return 'status--cancelled';
      case BookingStatus.Expired:
        return 'status--expired';
      default:
        return 'status--draft';
    }
  }

  getPaymentStatusLabel(status: PaymentTransactionStatus): string {
    switch (status) {
      case PaymentTransactionStatus.Unpaid:
        return 'Unpaid';
      case PaymentTransactionStatus.Pending:
        return 'Pending';
      case PaymentTransactionStatus.Paid:
        return 'Paid';
      case PaymentTransactionStatus.Failed:
        return 'Failed';
      case PaymentTransactionStatus.Refunded:
        return 'Refunded';
      case PaymentTransactionStatus.PartiallyRefunded:
        return 'Partially refunded';
      default:
        return 'Unknown';
    }
  }

  getPaymentStatusClass(status: PaymentTransactionStatus): string {
    switch (status) {
      case PaymentTransactionStatus.Paid:
        return 'payment-badge--paid';
      case PaymentTransactionStatus.Pending:
        return 'payment-badge--pending';
      case PaymentTransactionStatus.Failed:
        return 'payment-badge--failed';
      case PaymentTransactionStatus.Refunded:
      case PaymentTransactionStatus.PartiallyRefunded:
        return 'payment-badge--refunded';
      default:
        return 'payment-badge--unpaid';
    }
  }

  get effectivePaymentStatus(): PaymentTransactionStatus | null {
    if (this.booking?.paymentStatus == null) return null;
    return this.booking.paymentStatus;
  }

  private parseUtc(dateStr: string): Date {
    return new Date(dateStr.endsWith('Z') ? dateStr : `${dateStr}Z`);
  }

  formatDuration(minutes: number): string {
    if (minutes < 60) return `${minutes} min`;
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    return m > 0 ? `${h}h ${m}m` : `${h}h`;
  }
}
