import { Component, OnInit, inject, computed } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ServicePackagesApiService } from '../../../api-services/service-packages/service-packages-api.service';
import { BookingsService } from '../../../api-services/bookings/bookings-api.service';
import { VehiclesApiService } from '../../../api-services/vehicles/vehicles-api.service';
import { VehicleCategoriesApiService } from '../../../api-services/vehicle-categories/vehicle-categories-api.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { DialogButton, DialogType } from '../../shared/models/dialog-config.model';

import {
  ListServicePackagesQueryDto,
  GetAvailableAddonsQueryDto,
} from '../../../api-services/service-packages/service-packages-api.models';
import { ListMyVehiclesQueryDto } from '../../../api-services/vehicles/vehicles-api.model';
import { VehicleCategoryDto } from '../../../api-services/vehicle-categories/vehicle-categories-api.model';
import {
  ServiceMode,
  CreateBookingHoldCommand,
  AvailabilitySlotDto,
} from '../../../api-services/bookings/bookings-api.models';

@Component({
  selector: 'app-booking-wizard',
  standalone: false,
  templateUrl: './booking-wizard.component.html',
  styleUrl: './booking-wizard.component.scss',
})
export class BookingWizardComponent implements OnInit {
  private readonly currentUserService = inject(CurrentUserService);
  private readonly servicePackagesService = inject(ServicePackagesApiService);
  private readonly bookingsService = inject(BookingsService);
  private readonly vehiclesService = inject(VehiclesApiService);
  private readonly vehicleCategoriesService = inject(VehicleCategoriesApiService);
  private readonly dialogHelper = inject(DialogHelperService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  readonly isAuthenticated = computed(() => this.currentUserService.isAuthenticated());
  readonly isFleet = computed(() => this.currentUserService.isFleet());

  currentStep = 1;
  readonly stepLabels = ['Service', 'Vehicle', 'Date & Time', 'Confirm'];
  readonly stepIcons = ['auto_fix_high', 'directions_car', 'event', 'payment'];
  readonly dayLabels = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'];

  // Step 1
  packages: ListServicePackagesQueryDto[] = [];
  selectedPackage?: ListServicePackagesQueryDto;
  availableAddons: GetAvailableAddonsQueryDto[] = [];
  selectedAddonIds = new Set<number>();
  isLoadingPackages = false;
  isLoadingAddons = false;

  // Step 2
  vehicles: ListMyVehiclesQueryDto[] = [];
  selectedVehicleIds = new Set<number>();
  isLoadingVehicles = false;
  showAddVehicleForm = false;
  vehicleCategories: VehicleCategoryDto[] = [];
  addVehicleForm!: FormGroup;
  isAddingVehicle = false;
  addVehicleError?: string;

  // Step 3
  readonly serviceMode = ServiceMode.InShop;
  readonly shopLocationId = 1;
  calendarYear = new Date().getFullYear();
  calendarMonth = new Date().getMonth();
  calendarWeeks: (Date | null)[][] = [];
  selectedDate?: Date;
  availableSlots: AvailabilitySlotDto[] = [];
  selectedSlot?: AvailabilitySlotDto;
  isLoadingSlots = false;
  notes = '';

  // Step 4
  isSubmitting = false;
  submitError?: string;

  ngOnInit(): void {
    this.buildCalendar();
    this.loadPackages();
    this.addVehicleForm = this.fb.group({
      brand: ['', Validators.required],
      model: ['', Validators.required],
      yearOfManufacture: [
        new Date().getFullYear(),
        [
          Validators.required,
          Validators.min(1900),
          Validators.max(new Date().getFullYear() + 1),
        ],
      ],
      licencePlate: ['', Validators.required],
      vehicleCategoryId: [null, Validators.required],
    });
  }

  // ── Step 1: Packages ──────────────────────────────────────────────────────

  loadPackages(): void {
    this.isLoadingPackages = true;
    this.servicePackagesService.list().subscribe({
      next: (result) => {
        this.packages = result.items;
        this.isLoadingPackages = false;
      },
      error: () => {
        this.isLoadingPackages = false;
      },
    });
  }

  selectPackage(pkg: ListServicePackagesQueryDto): void {
    this.selectedPackage = pkg;
    this.selectedAddonIds.clear();
    this.availableAddons = [];
    this.isLoadingAddons = true;
    this.servicePackagesService.getAvailableAddons(pkg.id).subscribe({
      next: (result) => {
        this.availableAddons = result.items;
        this.isLoadingAddons = false;
      },
      error: () => {
        this.isLoadingAddons = false;
      },
    });
  }

  toggleAddon(addonId: number): void {
    if (this.selectedAddonIds.has(addonId)) {
      this.selectedAddonIds.delete(addonId);
    } else {
      this.selectedAddonIds.add(addonId);
    }
  }

  isAddonSelected(addonId: number): boolean {
    return this.selectedAddonIds.has(addonId);
  }

  // ── Navigation ────────────────────────────────────────────────────────────

  nextStep(): void {
    if (this.currentStep === 1) {
      if (!this.selectedPackage) return;
      if (!this.isAuthenticated()) {
        this.router.navigate(['/auth/login'], { queryParams: { returnUrl: '/book-now' } });
        return;
      }
      this.currentStep = 2;
      this.loadVehiclesIfNeeded();
      return;
    }
    if (this.currentStep === 2) {
      if (this.selectedVehicleIds.size === 0) return;
      this.currentStep = 3;
      return;
    }
    if (this.currentStep === 3) {
      if (!this.selectedSlot) return;
      this.currentStep = 4;
    }
  }

  prevStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  // ── Step 2: Vehicles ──────────────────────────────────────────────────────

  loadVehiclesIfNeeded(): void {
    if (this.vehicles.length === 0) {
      this.loadVehicles();
    }
  }

  loadVehicles(): void {
    this.isLoadingVehicles = true;
    this.vehiclesService.listMine().subscribe({
      next: (result) => {
        this.vehicles = result.items;
        this.isLoadingVehicles = false;
      },
      error: () => {
        this.isLoadingVehicles = false;
      },
    });
  }

  selectVehicle(vehicleId: number): void {
    if (this.isFleet()) {
      if (this.selectedVehicleIds.has(vehicleId)) {
        this.selectedVehicleIds.delete(vehicleId);
      } else {
        this.selectedVehicleIds.add(vehicleId);
      }
    } else {
      this.selectedVehicleIds.clear();
      this.selectedVehicleIds.add(vehicleId);
    }
  }

  isVehicleSelected(vehicleId: number): boolean {
    return this.selectedVehicleIds.has(vehicleId);
  }

  openAddVehicleForm(): void {
    if (this.vehicleCategories.length === 0) {
      this.vehicleCategoriesService.list().subscribe({
        next: (cats) => {
          this.vehicleCategories = cats;
        },
      });
    }
    this.showAddVehicleForm = true;
    this.addVehicleError = undefined;
    this.addVehicleForm.reset({ yearOfManufacture: new Date().getFullYear() });
  }

  cancelAddVehicle(): void {
    this.showAddVehicleForm = false;
  }

  submitAddVehicle(): void {
    if (this.addVehicleForm.invalid) {
      this.addVehicleForm.markAllAsTouched();
      return;
    }
    this.isAddingVehicle = true;
    this.addVehicleError = undefined;
    this.vehiclesService.create(this.addVehicleForm.value).subscribe({
      next: () => {
        this.isAddingVehicle = false;
        this.showAddVehicleForm = false;
        this.loadVehicles();
      },
      error: (err) => {
        this.isAddingVehicle = false;
        this.addVehicleError = err?.error?.message ?? 'Failed to add vehicle.';
      },
    });
  }

  // ── Step 3: Calendar & Availability ──────────────────────────────────────

  buildCalendar(): void {
    const firstDay = new Date(this.calendarYear, this.calendarMonth, 1);
    const lastDay = new Date(this.calendarYear, this.calendarMonth + 1, 0);
    const weeks: (Date | null)[][] = [];
    let week: (Date | null)[] = [];

    for (let i = 0; i < firstDay.getDay(); i++) week.push(null);

    for (let d = 1; d <= lastDay.getDate(); d++) {
      week.push(new Date(this.calendarYear, this.calendarMonth, d));
      if (week.length === 7) {
        weeks.push(week);
        week = [];
      }
    }
    if (week.length > 0) {
      while (week.length < 7) week.push(null);
      weeks.push(week);
    }
    this.calendarWeeks = weeks;
  }

  prevMonth(): void {
    if (this.calendarMonth === 0) {
      this.calendarMonth = 11;
      this.calendarYear--;
    } else {
      this.calendarMonth--;
    }
    this.buildCalendar();
  }

  nextMonth(): void {
    if (this.calendarMonth === 11) {
      this.calendarMonth = 0;
      this.calendarYear++;
    } else {
      this.calendarMonth++;
    }
    this.buildCalendar();
  }

  selectDate(date: Date): void {
    if (this.isPastDate(date)) return;
    this.selectedDate = date;
    this.selectedSlot = undefined;
    this.availableSlots = [];
    this.loadAvailability();
  }

  loadAvailability(): void {
    if (!this.selectedDate || !this.selectedPackage) return;
    const dateUtc = new Date(
      Date.UTC(
        this.selectedDate.getFullYear(),
        this.selectedDate.getMonth(),
        this.selectedDate.getDate(),
      ),
    ).toISOString();

    this.isLoadingSlots = true;
    this.bookingsService
      .getAvailability({
        dateUtc,
        servicePackageId: this.selectedPackage.id,
        addonItemIds: Array.from(this.selectedAddonIds),
        serviceMode: this.serviceMode,
        shopLocationId: this.shopLocationId,
      })
      .subscribe({
        next: (slots) => {
          this.availableSlots = slots;
          this.isLoadingSlots = false;
        },
        error: () => {
          this.isLoadingSlots = false;
        },
      });
  }

  selectSlot(slot: AvailabilitySlotDto): void {
    this.selectedSlot = slot;
  }

  // ── Step 4: Confirm & Submit ──────────────────────────────────────────────

  confirmBooking(): void {
    if (!this.selectedPackage || !this.selectedSlot || this.selectedVehicleIds.size === 0) return;

    this.isSubmitting = true;
    this.submitError = undefined;

    const command: CreateBookingHoldCommand = {
      servicePackageId: this.selectedPackage.id,
      addonItemIds: Array.from(this.selectedAddonIds),
      serviceMode: this.serviceMode,
      shopLocationId: this.shopLocationId,
      startUtc: this.selectedSlot.startUtc,
      vehicleIds: Array.from(this.selectedVehicleIds),
      notes: this.notes || undefined,
    };

    this.bookingsService.createHold(command).subscribe({
      next: (result) => {
        this.isSubmitting = false;
        this.showSuccessDialog(result.id);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.submitError = err?.error?.message ?? 'Failed to create booking. Please try again.';
      },
    });
  }

  private showSuccessDialog(bookingId: number): void {
    this.dialogHelper
      .open({
        type: DialogType.SUCCESS,
        title: 'Booking Created!',
        message: `Your ${this.selectedPackage?.name} appointment has been reserved. You have 10 minutes to complete payment before the slot is released.`,
        icon: 'check_circle',
        width: '480px',
        buttons: [
          { type: DialogButton.NO, label: 'View Appointments' },
          { type: DialogButton.YES, label: 'Pay Now', color: 'primary' },
        ],
      })
      .subscribe((result) => {
        if (result?.button === DialogButton.YES) {
          this.router.navigate(['/client/bookings', bookingId, 'pay']);
        } else {
          this.router.navigate(['/client/bookings']);
        }
      });
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  isPastDate(date: Date): boolean {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return date < today;
  }

  isToday(date: Date): boolean {
    return date.toDateString() === new Date().toDateString();
  }

  isSelectedDate(date: Date): boolean {
    return !!this.selectedDate && date.toDateString() === this.selectedDate.toDateString();
  }

  isSlotSelected(slot: AvailabilitySlotDto): boolean {
    return this.selectedSlot?.startUtc === slot.startUtc;
  }

  getMonthYear(): string {
    return new Date(this.calendarYear, this.calendarMonth, 1).toLocaleString('en-US', {
      month: 'long',
      year: 'numeric',
    });
  }

  formatSlotTime(dateStr: string): string {
    return new Date(dateStr).toLocaleString('en-US', {
      hour: 'numeric',
      minute: '2-digit',
      hour12: true,
    });
  }

  formatSelectedDate(): string {
    if (!this.selectedDate) return '';
    return this.selectedDate.toLocaleDateString('en-GB', {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    });
  }

  formatSelectedSlotTime(): string {
    return this.selectedSlot ? this.formatSlotTime(this.selectedSlot.startUtc) : '';
  }

  get totalPrice(): number {
    return (this.selectedPackage?.price ?? 0) + this.totalAddonPrice;
  }

  get totalAddonPrice(): number {
    return this.availableAddons
      .filter((a) => this.selectedAddonIds.has(a.id))
      .reduce((sum, a) => sum + a.price, 0);
  }

  get selectedAddonsList(): GetAvailableAddonsQueryDto[] {
    return this.availableAddons.filter((a) => this.selectedAddonIds.has(a.id));
  }

  get selectedVehiclesList(): ListMyVehiclesQueryDto[] {
    return this.vehicles.filter((v) => this.selectedVehicleIds.has(v.id));
  }

  get canProceedStep1(): boolean {
    return !!this.selectedPackage;
  }

  get canProceedStep2(): boolean {
    return this.selectedVehicleIds.size > 0;
  }

  get canProceedStep3(): boolean {
    return !!this.selectedSlot;
  }
}
