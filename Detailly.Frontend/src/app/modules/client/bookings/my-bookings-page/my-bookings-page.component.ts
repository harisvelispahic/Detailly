import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import {
  BookingStatus,
  ListMyBookingsQueryDto,
  ListMyBookingsRequest,
} from '../../../../api-services/bookings/bookings-api.models';
import { BookingsService } from '../../../../api-services/bookings/bookings-api.service';

@Component({
  selector: 'app-my-bookings-page',
  standalone: false,
  templateUrl: './my-bookings-page.component.html',
  styleUrl: './my-bookings-page.component.scss',
})
export class MyBookingsPageComponent
  extends BaseListPagedComponent<ListMyBookingsQueryDto, ListMyBookingsRequest>
  implements OnInit, OnDestroy
{
  private bookingsService = inject(BookingsService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  searchControl = new FormControl('');
  statusFilter: BookingStatus | null = null;
  BookingStatus = BookingStatus;

  readonly statusOptions: { value: BookingStatus | null; label: string }[] = [
    { value: null, label: 'All Status' },
    { value: BookingStatus.Confirmed, label: 'Confirmed' },
    { value: BookingStatus.PendingPayment, label: 'Pending Payment' },
    { value: BookingStatus.Completed, label: 'Completed' },
    { value: BookingStatus.Cancelled, label: 'Cancelled' },
    { value: BookingStatus.Expired, label: 'Expired' },
  ];

  constructor() {
    super();
    this.request = new ListMyBookingsRequest();
    this.request.paging.pageSize = 10;
  }

  ngOnInit(): void {
    this.initList();
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => {});
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected loadPagedData(): void {
    this.startLoading();
    this.bookingsService.listMine(this.request).subscribe({
      next: (result) => {
        this.handlePageResult(result);
        this.stopLoading();
      },
      error: () => {
        this.stopLoading('Failed to load appointments.');
      },
    });
  }

  reload(): void {
    this.loadPagedData();
  }

  get filteredItems(): ListMyBookingsQueryDto[] {
    let result = this.items;
    const search = this.searchControl.value?.toLowerCase().trim();
    if (search) {
      result = result.filter((b) => b.servicePackageName.toLowerCase().includes(search));
    }
    if (this.statusFilter !== null) {
      result = result.filter((b) => b.status === this.statusFilter);
    }
    return result;
  }

  get upcomingCount(): number {
    return this.items.filter((b) => b.status === BookingStatus.Confirmed).length;
  }

  get completedCount(): number {
    return this.items.filter((b) => b.status === BookingStatus.Completed).length;
  }

  get totalSpent(): number {
    return this.items
      .filter((b) => b.status === BookingStatus.Completed || b.status === BookingStatus.Confirmed)
      .reduce((sum, b) => sum + b.totalPrice, 0);
  }

  clearFilters(): void {
    this.searchControl.setValue('');
    this.statusFilter = null;
  }

  openDetails(id: number): void {
    this.router.navigate(['/client/bookings', id]);
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

  getDateColorClass(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Confirmed:
        return 'date-block--confirmed';
      case BookingStatus.Completed:
        return 'date-block--completed';
      case BookingStatus.Cancelled:
        return 'date-block--cancelled';
      case BookingStatus.PendingPayment:
        return 'date-block--pending';
      default:
        return 'date-block--default';
    }
  }

  getDateMonth(dateStr: string): string {
    return new Date(dateStr).toLocaleString('en-US', { month: 'short' }).toUpperCase();
  }

  getDateDay(dateStr: string): string {
    return new Date(dateStr).getDate().toString();
  }

}
