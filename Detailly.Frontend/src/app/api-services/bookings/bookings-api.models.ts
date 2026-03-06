import { PageResult } from '../../core/models/paging/page-result';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

export enum BookingStatus {
  Draft = 0, // User is building booking (no payment intent yet)
  PendingPayment = 1, // Payment intent created, waiting for payment
  Confirmed = 2, // Fully paid & accepted
  Cancelled = 3, // Cancelled (before or after payment)
  Completed = 4, // Service finished
  Expired = 5, // Service payment period expired
}

export enum PaymentTransactionStatus {
  Unpaid = 0, // Created internally, no payment started
  Pending = 1, // External provider processing
  Paid = 2, // Fully paid (atomic success)
  Failed = 3, // Payment failed
  Refunded = 4, // Full refund issued
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
