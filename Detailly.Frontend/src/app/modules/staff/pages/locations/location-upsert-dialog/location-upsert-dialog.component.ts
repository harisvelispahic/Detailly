import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { forkJoin } from 'rxjs';
import { LocationsApiService } from '../../../../../api-services/locations/locations-api.service';
import {
  CreateLocationCommand,
  GetLocationByIdDto,
  ListLocationsQueryDto,
  LocationOpeningHoursDto,
  LocationOpeningHoursInputDto,
  UpdateLocationCommand,
} from '../../../../../api-services/locations/locations-api.models';
import {
  ALL_COUNTRIES,
  CountryOption,
} from '../../../../client/profile/address-dialog/address-dialog.component';

export interface LocationUpsertDialogData {
  location: ListLocationsQueryDto | null;
}

const DAYS = [
  { label: 'Sunday',    short: 'Sun' },
  { label: 'Monday',    short: 'Mon' },
  { label: 'Tuesday',   short: 'Tue' },
  { label: 'Wednesday', short: 'Wed' },
  { label: 'Thursday',  short: 'Thu' },
  { label: 'Friday',    short: 'Fri' },
  { label: 'Saturday',  short: 'Sat' },
];

const HOURS = Array.from({ length: 24 }, (_, i) => i);
const MINUTES = [0, 15, 30, 45];

@Component({
  selector: 'app-location-upsert-dialog',
  standalone: false,
  templateUrl: './location-upsert-dialog.component.html',
  styleUrl: './location-upsert-dialog.component.scss',
})
export class LocationUpsertDialogComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isFetchingDetails = false;
  serverError: string | null = null;

  readonly days = DAYS;
  readonly hours = HOURS;
  readonly minutes = MINUTES;

  // Opening hours state: one entry per day (index = DayOfWeek)
  openingHours: Array<{
    isClosed: boolean;
    openHour: number | null;
    openMinute: number | null;
    closeHour: number | null;
    closeMinute: number | null;
  }> = DAYS.map(() => ({ isClosed: false, openHour: 8, openMinute: 0, closeHour: 17, closeMinute: 0 }));

  // Country autocomplete
  countrySearchCtrl = new FormControl<CountryOption | string>('');
  filteredCountries: CountryOption[] = [...ALL_COUNTRIES];

  displayCountry = (value: CountryOption | string | null): string => {
    if (!value) return '';
    if (typeof value === 'string') return value;
    return value.name;
  };

  get isEdit(): boolean {
    return !!this.data.location;
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<LocationUpsertDialogComponent>,
    private locationsApi: LocationsApiService,
    @Inject(MAT_DIALOG_DATA) public data: LocationUpsertDialogData,
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name:        ['', [Validators.required, Validators.maxLength(150)]],
      description: [''],
      totalBays:   [1, [Validators.required, Validators.min(1)]],
      // Address fields
      street:     ['', Validators.required],
      city:       ['', Validators.required],
      postalCode: ['', Validators.required],
      region:     [''],
      country:    ['', Validators.required],
    });

    this.countrySearchCtrl.valueChanges.subscribe((value) => {
      if (typeof value === 'string') {
        const search = value.trim().toLowerCase();
        this.filteredCountries = search
          ? ALL_COUNTRIES.filter(c => c.name.toLowerCase().includes(search))
          : [...ALL_COUNTRIES];
      } else {
        this.filteredCountries = [...ALL_COUNTRIES];
      }
    });

    if (this.isEdit) {
      this.loadLocationDetails();
    }
  }

  private loadLocationDetails(): void {
    const loc = this.data.location!;
    this.isFetchingDetails = true;

    forkJoin({
      detail: this.locationsApi.getById(loc.id),
      hours:  this.locationsApi.getOpeningHours(loc.id),
    }).subscribe({
      next: ({ detail, hours }) => {
        this.patchForm(detail, hours);
        this.isFetchingDetails = false;
      },
      error: () => {
        this.serverError = 'Failed to load location details.';
        this.isFetchingDetails = false;
      },
    });
  }

  private patchForm(detail: GetLocationByIdDto, hours: LocationOpeningHoursDto[]): void {
    const a = detail.address;
    this.form.patchValue({
      name:        detail.name,
      description: detail.description ?? '',
      totalBays:   detail.totalBays,
      street:      a.street ?? '',
      city:        a.city ?? '',
      postalCode:  a.postalCode ?? '',
      region:      a.region ?? '',
      country:     a.country ?? '',
    });

    if (a.country) {
      const match = ALL_COUNTRIES.find(c => c.name === a.country);
      this.countrySearchCtrl.setValue(match ?? a.country ?? '', { emitEvent: false });
    }

    // Patch opening hours
    this.openingHours = DAYS.map((_, i) => {
      const h = hours.find(h => h.dayOfWeek === i);
      return h
        ? { isClosed: h.isClosed, openHour: h.openHour, openMinute: h.openMinute, closeHour: h.closeHour, closeMinute: h.closeMinute }
        : { isClosed: false, openHour: 8, openMinute: 0, closeHour: 17, closeMinute: 0 };
    });
  }

  onCountryFocus(): void {
    this.countrySearchCtrl.setValue('', { emitEvent: true });
  }

  onCountryBlur(): void {
    setTimeout(() => {
      if (typeof this.countrySearchCtrl.value !== 'object') {
        const current = this.form.get('country')?.value;
        const match = ALL_COUNTRIES.find(c => c.name === current);
        this.countrySearchCtrl.setValue(match ?? current ?? '', { emitEvent: false });
      }
    }, 250);
  }

  onCountrySelected(option: CountryOption): void {
    this.form.patchValue({ country: option.name });
  }

  formatHour(h: number): string {
    return h.toString().padStart(2, '0');
  }

  formatMinute(m: number): string {
    return m.toString().padStart(2, '0');
  }

  private buildOpeningHoursPayload(): LocationOpeningHoursInputDto[] {
    return this.openingHours.map((h, i) => ({
      dayOfWeek:   i,
      isClosed:    h.isClosed,
      openHour:    h.isClosed ? null : (h.openHour ?? null),
      openMinute:  h.isClosed ? null : (h.openMinute ?? null),
      closeHour:   h.isClosed ? null : (h.closeHour ?? null),
      closeMinute: h.isClosed ? null : (h.closeMinute ?? null),
    }));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.serverError = null;
    this.isLoading = true;
    const v = this.form.value;

    if (this.isEdit) {
      const command: UpdateLocationCommand = {
        name:        v.name.trim(),
        description: v.description?.trim() || null,
        totalBays:   v.totalBays,
        address: {
          street:     v.street.trim(),
          city:       v.city.trim(),
          postalCode: v.postalCode.trim(),
          region:     v.region?.trim() || null,
          country:    v.country.trim(),
        },
        openingHours: this.buildOpeningHoursPayload(),
      };

      this.locationsApi.update(this.data.location!.id, command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.serverError = err?.error?.message ?? 'Update failed.';
          this.isLoading = false;
        },
      });
    } else {
      const command: CreateLocationCommand = {
        name:        v.name.trim(),
        description: v.description?.trim() || null,
        totalBays:   v.totalBays,
        address: {
          street:     v.street.trim(),
          city:       v.city.trim(),
          postalCode: v.postalCode.trim(),
          region:     v.region?.trim() || null,
          country:    v.country.trim(),
        },
        openingHours: this.buildOpeningHoursPayload(),
      };

      this.locationsApi.create(command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.serverError = err?.error?.message ?? 'Create failed.';
          this.isLoading = false;
        },
      });
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
