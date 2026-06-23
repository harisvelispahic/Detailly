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
  PartiallyRefunded = 5, // Refund issued for less than the full amount
}

// ---- Minimal DTOs (adjust fields to match your backend DTOs) ----
export interface ListMyBookingsQueryDto {
  id: number;
  status: BookingStatus;
  startUtc: string;
  endUtc: string;
  totalPrice: number;
  servicePackageName: string;
  servicePackageId: number;
  canRate: boolean;
}

export interface BookingAddressDto {
  street: string;
  city: string;
  postalCode?: string | null;
  country: string;
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
  shopLocationName: string;
  serviceAddress?: BookingAddressDto | null;
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

export interface GetAvailabilityResponse {
  slots: AvailabilitySlotDto[];
  travelTimeMinutes: number;
  mobileSurchargeFee: number;
}

export interface GetAvailabilityRequest {
  dateUtc: string;
  servicePackageId: number;
  addonItemIds?: number[];
  serviceMode: ServiceMode;
  shopLocationId: number;
  serviceAddressId?: number;
  vehicleIds?: number[];
}

// ---- Staff: Unassigned confirmed bookings (admin/manager) ----
export interface ListUnassignedBookingsQueryDto {
  id: number;
  startUtc: string;
  endUtc: string;
  requiredEmployees: number;
  serviceMode: ServiceMode;
  customerName: string;
  servicePackageName: string;
  totalPrice: number;
  notes?: string | null;
}

// ---- Employee: My assigned bookings ----
export interface ListMyAssignedBookingsQueryDto {
  id: number;
  status: BookingStatus;
  serviceMode: ServiceMode;
  startUtc: string;
  endUtc: string;
  totalPrice: number;
  servicePackageName: string;
  customerName: string;
  shopLocationName: string;
  notes?: string | null;
}

// ---- Assignable employees ----
export interface AssignableEmployeeDto {
  employeeId: number;
  fullName: string;
  hasOverlappingAssignment: boolean;
}

export interface AssignEmployeesCommand {
  employeeIds: number[];
}
