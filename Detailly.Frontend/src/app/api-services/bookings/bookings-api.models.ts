import { PageResult } from '../../core/models/paging/page-result';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

export enum BookingStatus {
  Draft = 1, // User is building booking (no payment intent yet)
  PendingPayment = 2, // Payment intent created, waiting for payment
  Confirmed = 3, // Fully paid & accepted
  Cancelled = 4, // Cancelled (before or after payment)
  Completed = 5, // Service finished
  Expired = 6, // Service payment period expired
}

export enum PaymentTransactionStatus {
  Unpaid = 1, // Created internally, no payment started
  Pending = 2, // External provider processing
  Paid = 3, // Fully paid (atomic success)
  Failed = 4, // Payment failed
  Refunded = 5, // Full refund issued
}

// ---- Minimal DTOs (adjust fields to match your backend DTOs) ----
export interface ListMyBookingsQueryDto {
  id: number;
  status: BookingStatus;
  startUtc: string;
  endUtc: string;
  totalPrice: number;
  servicePackageName: string;
}

export interface GetBookingByIdQueryDto {
  id: number;
  status: BookingStatus;
  startUtc: string;
  endUtc: string;
  totalPrice: number;
  requiredEmployees: number;
  requiredBays: number;
  reservationExpiresAtUtc?: string | null;
  notes?: string | null;
  servicePackageId: number;
  servicePackageName: string;
  paymentTransactionId?: number | null;
  paymentStatus?: PaymentTransactionStatus | null;

  addons?: BookingAddonDto[];
  vehicleIds: number[];
}

export interface BookingAddonDto {
  bookingItemId: number;
  name: string;
  priceSnapshot: number;
  durationMinutesSnapshot: number;
  requiredEmployeesSnapshot: number;
}

export interface CancelBookingCommand {
  reason?: string | null;
}
