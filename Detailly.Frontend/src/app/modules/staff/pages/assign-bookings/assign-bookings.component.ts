import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { BookingsService } from '../../../../api-services/bookings/bookings-api.service';
import {
  AssignableEmployeeDto,
  ListUnassignedBookingsQueryDto,
  ServiceMode,
} from '../../../../api-services/bookings/bookings-api.models';
import { ToasterService } from '../../../../core/services/toaster.service';
import { BasePagedQuery } from '../../../../core/models/paging/base-paged-query';

@Component({
  selector: 'app-assign-bookings',
  standalone: false,
  templateUrl: './assign-bookings.component.html',
  styleUrl: './assign-bookings.component.scss',
})
export class AssignBookingsComponent extends BaseComponent implements OnInit, OnDestroy {
  private bookingsApi = inject(BookingsService);
  private toaster = inject(ToasterService);
  private destroy$ = new Subject<void>();

  ServiceMode = ServiceMode;

  // Unassigned bookings list
  bookings: ListUnassignedBookingsQueryDto[] = [];
  totalBookings = 0;
  page = 1;
  pageSize = 10;

  // Selected booking
  selectedBooking: ListUnassignedBookingsQueryDto | null = null;

  // Assignable employees
  employees: AssignableEmployeeDto[] = [];
  isLoadingEmployees = false;
  employeesError: string | null = null;

  // Selection state
  selectedEmployeeIds: Set<number> = new Set();

  // Submit state
  isSubmitting = false;

  ngOnInit(): void {
    this.loadBookings();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadBookings(): void {
    this.startLoading();
    const query = new BasePagedQuery();
    query.paging.page = this.page;
    query.paging.pageSize = this.pageSize;

    this.bookingsApi.listUnassigned(query).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.bookings = result.items;
        this.totalBookings = result.total;
        this.stopLoading();
      },
      error: () => this.stopLoading('Failed to load bookings.'),
    });
  }

  selectBooking(booking: ListUnassignedBookingsQueryDto): void {
    this.selectedBooking = booking;
    this.selectedEmployeeIds.clear();
    this.loadAssignableEmployees(booking.id);
  }

  private loadAssignableEmployees(bookingId: number): void {
    this.isLoadingEmployees = true;
    this.employeesError = null;
    this.employees = [];

    this.bookingsApi.listAssignableEmployees(bookingId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.employees = result;
        this.isLoadingEmployees = false;
      },
      error: () => {
        this.employeesError = 'Failed to load assignable employees.';
        this.isLoadingEmployees = false;
      },
    });
  }

  toggleEmployee(employeeId: number): void {
    if (this.selectedEmployeeIds.has(employeeId)) {
      this.selectedEmployeeIds.delete(employeeId);
    } else {
      if (this.selectedBooking && this.selectedEmployeeIds.size >= this.selectedBooking.requiredEmployees) {
        return;
      }
      this.selectedEmployeeIds.add(employeeId);
    }
  }

  isEmployeeSelected(employeeId: number): boolean {
    return this.selectedEmployeeIds.has(employeeId);
  }

  get canSubmit(): boolean {
    return (
      this.selectedBooking !== null &&
      this.selectedEmployeeIds.size > 0 &&
      !this.isSubmitting
    );
  }

  submitAssignment(): void {
    if (!this.canSubmit || !this.selectedBooking) return;

    this.isSubmitting = true;
    this.bookingsApi
      .assignEmployees(this.selectedBooking.id, { employeeIds: Array.from(this.selectedEmployeeIds) })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toaster.success('Employees assigned successfully.');
          this.isSubmitting = false;
          this.selectedBooking = null;
          this.selectedEmployeeIds.clear();
          this.employees = [];
          this.loadBookings();
        },
        error: () => {
          this.toaster.error('Failed to assign employees. Please try again.');
          this.isSubmitting = false;
        },
      });
  }

  onPageChange(newPage: number): void {
    this.page = newPage;
    this.loadBookings();
  }

  getServiceModeLabel(mode: ServiceMode): string {
    return mode === ServiceMode.InShop ? 'In Shop' : 'Mobile';
  }

  getServiceModeIcon(mode: ServiceMode): string {
    return mode === ServiceMode.InShop ? 'store' : 'directions_car';
  }

  getDuration(booking: ListUnassignedBookingsQueryDto): string {
    const start = new Date(booking.startUtc);
    const end = new Date(booking.endUtc);
    const mins = Math.round((end.getTime() - start.getTime()) / 60000);
    const h = Math.floor(mins / 60);
    const m = mins % 60;
    return m > 0 ? `${h}h ${m}m` : `${h}h`;
  }

  get totalPages(): number {
    return Math.ceil(this.totalBookings / this.pageSize);
  }
}
