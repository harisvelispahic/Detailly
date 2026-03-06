import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingsService } from '../../../../api-services/bookings/bookings-api.service';
import { GetBookingByIdQueryDto } from '../../../../api-services/bookings/bookings-api.models';

@Component({
  selector: 'app-booking-details-page',
  templateUrl: './booking-details-page.component.html',
  standalone: false,
})
export class BookingDetailsPageComponent implements OnInit {
  id!: number;

  isLoading = false;
  isCancelling = false;

  error?: string;
  booking?: GetBookingByIdQueryDto;

  cancelReason: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingsService: BookingsService,
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.error = undefined;

    this.bookingsService.getById(this.id).subscribe({
      next: (res) => {
        this.booking = res;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load booking details.';
        this.isLoading = false;
      },
    });
  }

  cancel(): void {
    if (!confirm('Cancel this booking?')) return;

    this.isCancelling = true;
    this.error = undefined;

    this.bookingsService
      .cancelBooking(this.id, { reason: this.cancelReason?.trim() || null })
      .subscribe({
        next: () => {
          this.isCancelling = false;
          alert('✅ Booking cancelled.');
          this.load(); // refresh status
        },
        error: (err) => {
          this.isCancelling = false;
          // best-effort message
          this.error = err?.error?.message ?? 'Failed to cancel booking.';
        },
      });
  }

  back(): void {
    this.router.navigate(['/client/bookings']);
  }
}
