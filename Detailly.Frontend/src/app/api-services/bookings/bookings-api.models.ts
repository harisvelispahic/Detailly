import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

export class ListMyBookingsRequest extends BasePagedQuery {
  constructor() {
    super();
  }
}

export enum ServiceMode {
  InShop = 0,
  Mobile = 1,
}

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
  serviceMode: ServiceMode;
  startUtc: string;
  endUtc: string;
  totalPrice: number;
  mobileSurchargeFee?: number | null;
  fleetDiscountPercent?: number | null;
  requiredEmployees: number;
  requiredBays: number;
  travelTimeMinutes?: number | null;
  departureUtc?: string | null;
  returnUtc?: string | null;
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

export interface CreateBookingHoldCommand {
  servicePackageId: number;
  addonItemIds?: number[];
  serviceMode: ServiceMode;
  shopLocationId: number;
  serviceAddressId?: number;
  startUtc: string;
  vehicleIds?: number[];
  notes?: string;
}

export interface AvailabilitySlotDto {
  startUtc: string;
  endUtc: string;
}

export interface GetAvailabilityRequest {
  dateUtc: string;
  servicePackageId: number;
  addonItemIds?: number[];
  serviceMode: ServiceMode;
  shopLocationId: number;
}
