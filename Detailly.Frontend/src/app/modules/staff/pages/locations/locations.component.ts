import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ListLocationsQueryDto, ListLocationsRequest } from '../../../../api-services/locations/locations-api.models';
import { LocationsApiService } from '../../../../api-services/locations/locations-api.service';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import { LocationUpsertDialogComponent, LocationUpsertDialogData } from './location-upsert-dialog/location-upsert-dialog.component';
import { LocationDetailDialogComponent, LocationDetailDialogData } from './location-detail-dialog/location-detail-dialog.component';

@Component({
  selector: 'app-locations',
  standalone: false,
  templateUrl: './locations.component.html',
  styleUrl: './locations.component.scss',
})
export class LocationsComponent
  extends BaseListPagedComponent<ListLocationsQueryDto, ListLocationsRequest>
  implements OnInit, OnDestroy
{
  private locationsApi = inject(LocationsApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  readonly auth = inject(AuthFacadeService);
  private destroy$ = new Subject<void>();

  displayedColumns = ['name', 'address', 'bays', 'status', 'actions'];
  searchCtrl = new FormControl('');

  constructor() {
    super();
    this.request = new ListLocationsRequest();
  }

  ngOnInit(): void {
    this.searchCtrl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$),
    ).subscribe((val) => {
      this.request.search = val?.trim() || null;
      this.request.paging.page = 1;
      this.loadPagedData();
    });

    this.loadPagedData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected loadPagedData(): void {
    this.startLoading();
    this.locationsApi.list(this.request).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.handlePageResult(res);
        this.stopLoading();
      },
      error: () => this.stopLoading('Failed to load locations.'),
    });
  }

  formatAddress(loc: ListLocationsQueryDto): string {
    const parts = [loc.street, loc.city, loc.postalCode, loc.region, loc.country]
      .filter((p): p is string => !!p);
    return parts.join(', ') || '—';
  }

  openCreateDialog(): void {
    this.dialog
      .open(LocationUpsertDialogComponent, {
        width: '700px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: { location: null } as LocationUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Location created.');
          this.loadPagedData();
        }
      });
  }

  openEditDialog(loc: ListLocationsQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialog
      .open(LocationUpsertDialogComponent, {
        width: '700px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: { location: loc } as LocationUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Location updated.');
          this.loadPagedData();
        }
      });
  }

  openDetailDialog(loc: ListLocationsQueryDto): void {
    this.dialog.open(LocationDetailDialogComponent, {
      width: '640px',
      maxWidth: '95vw',
      data: { locationId: loc.id, locationName: loc.name } as LocationDetailDialogData,
    });
  }

  toggleLocationStatus(loc: ListLocationsQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    const action = loc.isTemporarilyClosed ? 'reopen' : 'temporarily close';
    const message = loc.isTemporarilyClosed
      ? `Are you sure you want to reopen "${loc.name}"?`
      : `Are you sure you want to temporarily close "${loc.name}"? Customers won't be able to book at this location.`;

    this.dialogHelper.confirm(
      loc.isTemporarilyClosed ? 'Reopen Location' : 'Close Location',
      message,
      loc.isTemporarilyClosed ? 'store' : 'construction',
    ).subscribe((result) => {
      if (result?.button !== DialogButton.YES) return;
      this.locationsApi.toggleStatus(loc.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.toaster.success(loc.isTemporarilyClosed ? 'Location reopened.' : 'Location closed.');
          this.loadPagedData();
        },
        error: () => this.toaster.error(`Failed to ${action} location.`),
      });
    });
  }

  deleteLocation(loc: ListLocationsQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialogHelper.confirmDelete(loc.name).subscribe((result) => {
      if (result?.button !== DialogButton.DELETE) return;
      this.locationsApi.delete(loc.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.toaster.success('Location deleted.');
          this.loadPagedData();
        },
        error: () => this.toaster.error('Failed to delete location.'),
      });
    });
  }
}
