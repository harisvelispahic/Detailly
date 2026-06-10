import { Component, inject, Inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CreateEmployeeShiftCommand,
  EmployeeShiftDto,
  EmployeeWorkMode,
  UpdateEmployeeShiftCommand,
} from '../../../../../api-services/employee-shifts/employee-shifts-api.models';
import { EmployeeShiftsApiService } from '../../../../../api-services/employee-shifts/employee-shifts-api.service';
import {
  LocationDto,
  LocationOpeningHoursDto,
} from '../../../../../api-services/locations/locations-api.models';
import { LocationsApiService } from '../../../../../api-services/locations/locations-api.service';
import {
  EmployeeDto,
  ListAvailableEmployeesForShiftRequest,
} from '../../../../../api-services/staff-members/staff-members-api.models';
import { StaffMembersApiService } from '../../../../../api-services/staff-members/staff-members-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { extractHttpError } from '../../../../../core/utils/http-error.util';

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
  private staffMembersApi = inject(StaffMembersApiService);
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

  get selectedLocation(): LocationDto | undefined {
    const id = this.form?.get('shopLocationId')?.value;
    return id ? this.data.locations.find((l) => l.id === id) : undefined;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: ShiftUpsertDialogData) {}

  ngOnInit(): void {
    const shift = this.data.shift;

    const initialDate = shift
      ? this.utcIsoToLocalDate(shift.startUtc)
      : (this.data.date ?? new Date());

    // Field order: location → date → times → employee → work mode
    this.form = this.fb.group(
      {
        shopLocationId: [
          shift?.shopLocationId ?? this.data.locationId ?? null,
          Validators.required,
        ],
        date: [initialDate, Validators.required],
        startTime: [
          shift ? this.utcIsoToLocalTime(shift.startUtc) : '09:00',
          [Validators.required, Validators.pattern(TIME_PATTERN)],
        ],
        endTime: [
          shift ? this.utcIsoToLocalTime(shift.endUtc) : '17:00',
          [Validators.required, Validators.pattern(TIME_PATTERN)],
        ],
        employeeId: [shift?.employeeId ?? null, Validators.required],
        employeeWorkMode: [shift?.employeeWorkMode ?? EmployeeWorkMode.InShop, Validators.required],
      },
      { validators: [endAfterStartValidator, this.buildOpeningHoursValidator()] },
    );

    // Location change → reload opening hours and apply defaults to times
    this.form.get('shopLocationId')!.valueChanges.subscribe((locationId: number) => {
      if (locationId) this.loadAndApplyOpeningHours(locationId);
    });

    // Date change → re-apply opening hours for the new weekday, reload employee availability
    this.form.get('date')!.valueChanges.subscribe((date: Date) => {
      if (date) {
        if (!this.isEditMode) {
          this.applyTodayHours();
          this.form.patchValue({ employeeId: null }, { emitEvent: false });
        }
        this.loadEmployees();
      }
    });

    // Initial loads
    this.loadEmployees();

    const initialLocationId = shift?.shopLocationId ?? this.data.locationId;
    if (initialLocationId) {
      this.loadAndApplyOpeningHours(initialLocationId);
    }
  }

  private loadEmployees(): void {
    const request = new ListAvailableEmployeesForShiftRequest();
    const date: Date | null = this.form.get('date')?.value ?? null;
    if (date) {
      request.dateUtc = this.formatLocalDate(date) + 'T00:00:00.000Z';
    }
    if (this.isEditMode && this.data.shift) {
      request.excludeShiftId = this.data.shift.id;
    }

    this.isLoadingEmployees = true;
    this.form.get('employeeId')?.disable();
    this.staffMembersApi.listAvailableForShift(request).subscribe({
      next: (res) => {
        this.employees = res.items;
        this.isLoadingEmployees = false;
        this.form.get('employeeId')?.enable();
      },
      error: () => {
        this.isLoadingEmployees = false;
        this.form.get('employeeId')?.enable();
        this.toaster.error('Failed to load employees');
      },
    });
  }

  private loadAndApplyOpeningHours(locationId: number): void {
    this.locationsApi.getOpeningHours(locationId).subscribe({
      next: (hours) => {
        this.openingHoursCache = hours;
        if (!this.isEditMode) this.applyTodayHours();
        this.form.updateValueAndValidity();
      },
    });
  }

  private buildOpeningHoursValidator(): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {
      if (!this.openingHoursCache.length) return null;

      const date: Date | null = group.get('date')?.value ?? null;
      const startTime: string = group.get('startTime')?.value ?? '';
      const endTime: string = group.get('endTime')?.value ?? '';

      if (!date || !startTime || !endTime) return null;
      if (!TIME_PATTERN.test(startTime) || !TIME_PATTERN.test(endTime)) return null;

      const dayOfWeek = date.getDay();
      const hours = this.openingHoursCache.find((h) => h.dayOfWeek === dayOfWeek);

      if (!hours) return null;
      if (hours.isClosed) return { locationClosed: true };

      if (
        hours.openHour == null ||
        hours.openMinute == null ||
        hours.closeHour == null ||
        hours.closeMinute == null
      )
        return null;

      const [startH, startM] = startTime.split(':').map(Number);
      const [endH, endM] = endTime.split(':').map(Number);

      const shiftStartMins = startH * 60 + startM;
      const shiftEndMins = endH * 60 + endM;
      const openMins = hours.openHour * 60 + hours.openMinute;
      const closeMins = hours.closeHour * 60 + hours.closeMinute;

      if (shiftStartMins < openMins)
        return {
          shiftBeforeOpen: { openTime: this.formatHhmm(hours.openHour, hours.openMinute) },
        };
      if (shiftEndMins > closeMins)
        return {
          shiftAfterClose: { closeTime: this.formatHhmm(hours.closeHour, hours.closeMinute) },
        };

      return null;
    };
  }

  private applyTodayHours(): void {
    const selectedDate: Date = this.form.value.date ?? new Date();
    const dayOfWeek = selectedDate.getDay(); // 0=Sunday
    const todayHours = this.openingHoursCache.find((h) => h.dayOfWeek === dayOfWeek);

    if (!todayHours || todayHours.isClosed) return;

    if (todayHours.openHour != null && todayHours.openMinute != null) {
      this.form.patchValue(
        { startTime: this.formatHhmm(todayHours.openHour, todayHours.openMinute) },
        { emitEvent: false },
      );
    }
    if (todayHours.closeHour != null && todayHours.closeMinute != null) {
      this.form.patchValue(
        { endTime: this.formatHhmm(todayHours.closeHour, todayHours.closeMinute) },
        { emitEvent: false },
      );
    }
  }

  onSave(): void {
    if (this.form.invalid || this.isSubmitting) return;

    const { employeeId, shopLocationId, employeeWorkMode, date, startTime, endTime } =
      this.form.value;

    // Build local datetimes and convert to UTC ISO via toISOString()
    const localDate = new Date(date);
    const [startH, startM] = (startTime as string).split(':').map(Number);
    const [endH, endM] = (endTime as string).split(':').map(Number);

    const startLocal = new Date(
      localDate.getFullYear(),
      localDate.getMonth(),
      localDate.getDate(),
      startH,
      startM,
      0,
    );
    const endLocal = new Date(
      localDate.getFullYear(),
      localDate.getMonth(),
      localDate.getDate(),
      endH,
      endM,
      0,
    );

    const startUtc = startLocal.toISOString();
    const endUtc = endLocal.toISOString();

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
          this.toaster.error(extractHttpError(err) ?? 'Failed to update shift');
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
          this.toaster.error(extractHttpError(err) ?? 'Failed to create shift');
        },
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private utcIsoToLocalTime(isoStr: string): string {
    const d = new Date(isoStr);
    return `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;
  }

  private utcIsoToLocalDate(isoStr: string): Date {
    return new Date(isoStr);
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

}
