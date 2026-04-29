import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { forkJoin } from 'rxjs';
import { LocationsApiService } from '../../../../../api-services/locations/locations-api.service';
import { GetLocationByIdDto, LocationOpeningHoursDto } from '../../../../../api-services/locations/locations-api.models';

export interface LocationDetailDialogData {
  locationId: number;
  locationName: string;
}

const DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

@Component({
  selector: 'app-location-detail-dialog',
  standalone: false,
  templateUrl: './location-detail-dialog.component.html',
  styleUrl: './location-detail-dialog.component.scss',
})
export class LocationDetailDialogComponent implements OnInit {
  isLoading = true;
  error: string | null = null;

  detail: GetLocationByIdDto | null = null;
  hours: LocationOpeningHoursDto[] = [];

  readonly dayNames = DAY_NAMES;

  constructor(
    private locationsApi: LocationsApiService,
    private dialogRef: MatDialogRef<LocationDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: LocationDetailDialogData,
  ) {}

  ngOnInit(): void {
    forkJoin({
      detail: this.locationsApi.getById(this.data.locationId),
      hours:  this.locationsApi.getOpeningHours(this.data.locationId),
    }).subscribe({
      next: ({ detail, hours }) => {
        this.detail  = detail;
        this.hours   = this.buildFullWeek(hours);
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load location details.';
        this.isLoading = false;
      },
    });
  }

  /** Ensure all 7 days are present; fill missing days with isClosed=true. */
  private buildFullWeek(hours: LocationOpeningHoursDto[]): LocationOpeningHoursDto[] {
    return DAY_NAMES.map((_, i) => {
      const h = hours.find(h => h.dayOfWeek === i);
      return h ?? { dayOfWeek: i, isClosed: true, openHour: null, openMinute: null, closeHour: null, closeMinute: null };
    });
  }

  formatTime(hour: number | null, minute: number | null): string {
    if (hour === null) return '—';
    return `${hour.toString().padStart(2, '0')}:${(minute ?? 0).toString().padStart(2, '0')}`;
  }

  formatAddress(d: GetLocationByIdDto): string {
    const a = d.address;
    return [a.street, a.city, a.postalCode, a.region, a.country]
      .filter((p): p is string => !!p)
      .join(', ') || '—';
  }

  close(): void {
    this.dialogRef.close();
  }
}
