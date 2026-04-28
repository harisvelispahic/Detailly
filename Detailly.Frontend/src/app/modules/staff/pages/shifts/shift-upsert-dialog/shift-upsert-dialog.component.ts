import { Component, inject, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CreateEmployeeShiftCommand,
  EmployeeShiftDto,
  EmployeeWorkMode,
  UpdateEmployeeShiftCommand,
} from '../../../../../api-services/employee-shifts/employee-shifts-api.models';
import { EmployeeShiftsApiService } from '../../../../../api-services/employee-shifts/employee-shifts-api.service';
import { LocationDto, LocationOpeningHoursDto } from '../../../../../api-services/locations/locations-api.models';
import { LocationsApiService } from '../../../../../api-services/locations/locations-api.service';
import { EmployeeDto } from '../../../../../api-services/employees/employees-api.models';
import { EmployeesApiService } from '../../../../../api-services/employees/employees-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';

export interface ShiftUpsertDialogData {
  shift: EmployeeShiftDto | null;
  locationId: number | null;
  date: Date | null;
  locations: LocationDto[];
}

function endAfterStartValidator(group: AbstractControl): ValidationErrors | null {
  const start = group.get('startTime')?.value as string;
  const end = group.get('endTime')?.value as string;
  if (start && end && end <= start) {
    return { endBeforeStart: true };
  }
  return null;
}

const TIME_PATTERN = /^([01]\d|2[0-3]):[0-5]\d$/;

@Component({
  selector: 'app-shift-upsert-dialog',
  standalone: false,
  templateUrl: './shift-upsert-dialog.component.html',
  styleUrl: './shift-upsert-dialog.component.scss',
})
export class ShiftUpsertDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private shiftsApi = inject(EmployeeShiftsApiService);
  private locationsApi = inject(LocationsApiService);
  private employeesApi = inject(EmployeesApiService);
  private toaster = inject(ToasterService);
  private dialogRef = inject(MatDialogRef<ShiftUpsertDialogComponent>);

  form!: FormGroup;
  isSubmitting = false;
  EmployeeWorkMode = EmployeeWorkMode;

  employees: EmployeeDto[] = [];
  isLoadingEmployees = false;

  private openingHoursCache: LocationOpeningHoursDto[] = [];

  get isEditMode(): boolean {
    return this.data.shift !== null;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: ShiftUpsertDialogData) {}

  ngOnInit(): void {
    const shift = this.data.shift;

    const initialDate = shift
      ? this.utcIsoToLocalDate(shift.startUtc)
      : (this.data.date ?? new Date());

    this.form = this.fb.group(
      {
        employeeId: [shift?.employeeId ?? null, Validators.required],
        shopLocationId: [shift?.shopLocationId ?? this.data.locationId ?? null, Validators.required],
        employeeWorkMode: [shift?.employeeWorkMode ?? EmployeeWorkMode.InShop, Validators.required],
        date: [initialDate, Validators.required],
        startTime: [
          shift ? this.extractUtcTime(shift.startUtc) : '09:00',
          [Validators.required, Validators.pattern(TIME_PATTERN)],
        ],
        endTime: [
          shift ? this.extractUtcTime(shift.endUtc) : '17:00',
          [Validators.required, Validators.pattern(TIME_PATTERN)],
        ],
      },
      { validators: endAfterStartValidator },
    );

    this.loadEmployees();

    // When location changes, update default times from opening hours
    this.form.get('shopLocationId')!.valueChanges.subscribe((locationId: number) => {
      if (locationId) this.loadAndApplyOpeningHours(locationId);
    });

    // Load opening hours for pre-selected location
    const initialLocationId = shift?.shopLocationId ?? this.data.locationId;
    if (initialLocationId) {
      this.loadOpeningHours(initialLocationId);
    }
  }

  private loadEmployees(): void {
    this.isLoadingEmployees = true;
    this.employeesApi.list().subscribe({
      next: (list) => {
        this.employees = list;
        this.isLoadingEmployees = false;
      },
      error: () => {
        this.isLoadingEmployees = false;
        this.toaster.error('Failed to load employees');
      },
    });
  }

  private loadAndApplyOpeningHours(locationId: number): void {
    this.locationsApi.getOpeningHours(locationId).subscribe({
      next: (hours) => {
        this.openingHoursCache = hours;
        if (!this.isEditMode) this.applyTodayHours();
      },
    });
  }

  private loadOpeningHours(locationId: number): void {
    this.locationsApi.getOpeningHours(locationId).subscribe({
      next: (hours) => {
        this.openingHoursCache = hours;
        // Only apply defaults when creating — not when editing
        if (!this.isEditMode) this.applyTodayHours();
      },
    });
  }

  private applyTodayHours(): void {
    const selectedDate: Date = this.form.value.date ?? new Date();
    const dayOfWeek = selectedDate.getDay(); // 0=Sunday
    const todayHours = this.openingHoursCache.find((h) => h.dayOfWeek === dayOfWeek);

    if (!todayHours || todayHours.isClosed) return;

    if (todayHours.openHour != null && todayHours.openMinute != null) {
      const start = this.formatHhmm(todayHours.openHour, todayHours.openMinute);
      this.form.patchValue({ startTime: start }, { emitEvent: false });
    }
    if (todayHours.closeHour != null && todayHours.closeMinute != null) {
      const end = this.formatHhmm(todayHours.closeHour, todayHours.closeMinute);
      this.form.patchValue({ endTime: end }, { emitEvent: false });
    }
  }

  onSave(): void {
    if (this.form.invalid || this.isSubmitting) return;

    const { employeeId, shopLocationId, employeeWorkMode, date, startTime, endTime } =
      this.form.value;

    const dateStr = this.formatLocalDate(new Date(date));
    const startUtc = `${dateStr}T${startTime}:00.000Z`;
    const endUtc = `${dateStr}T${endTime}:00.000Z`;

    this.isSubmitting = true;

    if (this.isEditMode) {
      const command: UpdateEmployeeShiftCommand = {
        employeeId,
        shopLocationId,
        employeeWorkMode,
        startUtc,
        endUtc,
      };
      this.shiftsApi.update(this.data.shift!.id, command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.toaster.error(this.extractError(err) ?? 'Failed to update shift');
        },
      });
    } else {
      const command: CreateEmployeeShiftCommand = {
        employeeId,
        shopLocationId,
        employeeWorkMode,
        startUtc,
        endUtc,
      };
      this.shiftsApi.create(command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.toaster.error(this.extractError(err) ?? 'Failed to create shift');
        },
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private extractUtcTime(isoStr: string): string {
    return isoStr.substring(11, 16);
  }

  private utcIsoToLocalDate(isoStr: string): Date {
    const [y, m, d] = isoStr.substring(0, 10).split('-').map(Number);
    return new Date(y, m - 1, d);
  }

  private formatLocalDate(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  private formatHhmm(hour: number, minute: number): string {
    return `${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`;
  }

  private extractError(err: any): string | null {
    return err?.error?.message ?? err?.error?.title ?? err?.message ?? null;
  }
}
