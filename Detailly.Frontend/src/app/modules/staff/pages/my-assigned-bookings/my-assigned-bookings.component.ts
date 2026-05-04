import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BaseListComponent } from '../../../../core/components/base-classes/base-list-component';
import { BookingsService } from '../../../../api-services/bookings/bookings-api.service';
import {
  BookingStatus,
  ListMyAssignedBookingsQueryDto,
  ServiceMode,
} from '../../../../api-services/bookings/bookings-api.models';
import { ToasterService } from '../../../../core/services/toaster.service';
import { BasePagedQuery } from '../../../../core/models/paging/base-paged-query';

@Component({
  selector: 'app-my-bookings',
  standalone: false,
  templateUrl: './my-assigned-bookings.component.html',
  styleUrl: './my-assigned-bookings.component.scss',
})
export class MyAssignedBookingsComponent
  extends BaseListComponent<ListMyAssignedBookingsQueryDto>
  implements OnInit, OnDestroy
{
  private bookingsApi = inject(BookingsService);
  private toaster = inject(ToasterService);
  private destroy$ = new Subject<void>();

  BookingStatus = BookingStatus;
  ServiceMode = ServiceMode;

  displayedColumns = ['date', 'time', 'customer', 'package', 'location', 'status', 'actions'];

  total = 0;
  page = 1;
  pageSize = 10;

  completingId: number | null = null;

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected loadData(): void {
    this.startLoading();
    const query = new BasePagedQuery();
    query.paging.page = this.page;
    query.paging.pageSize = this.pageSize;

    this.bookingsApi
      .listMyAssigned(query)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.items = result.items;
          this.total = result.total;
          this.stopLoading();
        },
        error: () => this.stopLoading('Failed to load your assigned bookings.'),
      });
  }

  completeBooking(booking: ListMyAssignedBookingsQueryDto): void {
    this.completingId = booking.id;
    this.bookingsApi
      .completeBooking(booking.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toaster.success('Booking marked as completed.');
          this.completingId = null;
          this.loadData();
        },
        error: (err) => {
          const message = err?.error?.message ?? 'Failed to complete booking.';
          this.toaster.error(message);
          this.completingId = null;
        },
      });
  }

  onPageChange(newPage: number): void {
    this.page = newPage;
    this.loadData();
  }

  getStatusLabel(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Confirmed:
        return 'Confirmed';
      case BookingStatus.Completed:
        return 'Completed';
      default:
        return 'Unknown';
    }
  }

  getStatusClass(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Confirmed:
        return 'status-confirmed';
      case BookingStatus.Completed:
        return 'status-completed';
      default:
        return '';
    }
  }

  getServiceModeLabel(mode: ServiceMode): string {
    return mode === ServiceMode.InShop ? 'In Shop' : 'Mobile';
  }

  isCompletable(booking: ListMyAssignedBookingsQueryDto): boolean {
    return booking.status === BookingStatus.Confirmed && new Date() >= new Date(booking.endUtc);
  }

  get totalPages(): number {
    return Math.ceil(this.total / this.pageSize);
  }
}
