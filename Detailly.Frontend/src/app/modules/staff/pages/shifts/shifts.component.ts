import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import {
  EmployeeShiftDto,
  EmployeeWorkMode,
  ListEmployeeShiftsRequest,
} from '../../../../api-services/employee-shifts/employee-shifts-api.models';
import { EmployeeShiftsApiService } from '../../../../api-services/employee-shifts/employee-shifts-api.service';
import { LocationDto, ListLocationsRequest } from '../../../../api-services/locations/locations-api.models';
import { LocationsApiService } from '../../../../api-services/locations/locations-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import { ShiftUpsertDialogComponent, ShiftUpsertDialogData } from './shift-upsert-dialog/shift-upsert-dialog.component';
import { ShiftsExportDialogComponent, ShiftsExportDialogData } from './shifts-export-dialog/shifts-export-dialog.component';

@Component({
  selector: 'app-shifts',
  standalone: false,
  templateUrl: './shifts.component.html',
  styleUrl: './shifts.component.scss',
})
export class ShiftsComponent
  extends BaseListPagedComponent<EmployeeShiftDto, ListEmployeeShiftsRequest>
  implements OnInit, OnDestroy
{
  private shiftsApi = inject(EmployeeShiftsApiService);
  private locationsApi = inject(LocationsApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private destroy$ = new Subject<void>();

  displayedColumns = ['employee', 'workMode', 'start', 'end', 'duration', 'actions'];
  EmployeeWorkMode = EmployeeWorkMode;

  locations: LocationDto[] = [];
  isLoadingLocations = false;

  selectedDate: Date = new Date();
  selectedLocationId: number | null = null;
  workModeFilter: EmployeeWorkMode | null = null;

  constructor() {
    super();
    this.request = new ListEmployeeShiftsRequest();
  }

  ngOnInit(): void {
    this.loadLocations();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadLocations(): void {
    this.isLoadingLocations = true;
    const req = new ListLocationsRequest();
    this.locationsApi.list(req).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.locations = res.items;
        this.isLoadingLocations = false;
      },
      error: () => {
        this.isLoadingLocations = false;
        this.toaster.error('Failed to load locations');
      },
    });
  }

  protected loadPagedData(): void {
    if (!this.selectedLocationId) {
      this.items = [];
      this.totalItems = 0;
      return;
    }

    this.request.dateUtc = this.formatLocalDate(this.selectedDate) + 'T00:00:00.000Z';
    this.request.shopLocationId = this.selectedLocationId;
    this.request.employeeWorkMode = this.workModeFilter;

    this.startLoading();
    this.shiftsApi.list(this.request).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.handlePageResult(res);
        this.stopLoading();
      },
      error: () => this.stopLoading('Failed to load shifts'),
    });
  }

  onDateChange(date: Date | null): void {
    if (date) {
      this.selectedDate = date;
      this.request.paging.page = 1;
      this.loadPagedData();
    }
  }

  onLocationChange(locationId: number): void {
    this.selectedLocationId = locationId;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onWorkModeChange(mode: EmployeeWorkMode | null): void {
    this.workModeFilter = mode;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  openExportDialog(): void {
    if (!this.selectedLocationId) return;
    const locationName = this.locations.find(l => l.id === this.selectedLocationId)?.name ?? '';
    const data: ShiftsExportDialogData = {
      shopLocationId: this.selectedLocationId,
      locationName,
      workModeFilter: this.workModeFilter,
    };
    this.dialog.open(ShiftsExportDialogComponent, { width: '520px', maxWidth: '95vw', data });
  }

  openCreateDialog(): void {
    const data: ShiftUpsertDialogData = {
      shift: null,
      locationId: this.selectedLocationId,
      date: this.selectedDate,
      locations: this.locations,
    };
    this.dialog
      .open(ShiftUpsertDialogComponent, { width: '520px', maxWidth: '95vw', data })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Shift created');
          this.loadPagedData();
        }
      });
  }

  openEditDialog(shift: EmployeeShiftDto, event: MouseEvent): void {
    event.stopPropagation();
    const data: ShiftUpsertDialogData = {
      shift,
      locationId: shift.shopLocationId,
      date: new Date(shift.startUtc),
      locations: this.locations,
    };
    this.dialog
      .open(ShiftUpsertDialogComponent, { width: '520px', maxWidth: '95vw', data })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Shift updated');
          this.loadPagedData();
        }
      });
  }

  deleteShift(shift: EmployeeShiftDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialogHelper
      .confirmDelete(`shift for ${shift.employeeName}`)
      .subscribe((result) => {
        if (result?.button !== DialogButton.DELETE) return;
        this.shiftsApi.delete(shift.id).pipe(takeUntil(this.destroy$)).subscribe({
          next: () => {
            this.toaster.success('Shift deleted');
            this.loadPagedData();
          },
          error: () => this.toaster.error('Failed to delete shift'),
        });
      });
  }

  getWorkModeLabel(mode: EmployeeWorkMode): string {
    return mode === EmployeeWorkMode.InShop ? 'In Shop' : 'Mobile';
  }

  getWorkModeIcon(mode: EmployeeWorkMode): string {
    return mode === EmployeeWorkMode.InShop ? 'store' : 'directions_car';
  }

  getDuration(shift: EmployeeShiftDto): string {
    const start = new Date(shift.startUtc);
    const end = new Date(shift.endUtc);
    const totalMinutes = Math.round((end.getTime() - start.getTime()) / 60000);
    const hours = Math.floor(totalMinutes / 60);
    const mins = totalMinutes % 60;
    return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
  }

  get hasFilters(): boolean {
    return !!this.selectedLocationId || this.workModeFilter !== null;
  }

  private formatLocalDate(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }
}
