import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import {
  ListVehicleCategoriesQueryDto,
  ListVehicleCategoriesRequest,
} from '../../../../api-services/vehicle-categories/vehicle-categories-api.model';
import { VehicleCategoriesApiService } from '../../../../api-services/vehicle-categories/vehicle-categories-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import {
  VehicleCategoryUpsertDialogComponent,
  VehicleCategoryUpsertDialogData,
} from './vehicle-category-upsert-dialog/vehicle-category-upsert-dialog.component';

@Component({
  selector: 'app-vehicle-categories',
  standalone: false,
  templateUrl: './vehicle-categories.component.html',
  styleUrl: './vehicle-categories.component.scss',
})
export class VehicleCategoriesComponent
  extends BaseListPagedComponent<ListVehicleCategoriesQueryDto, ListVehicleCategoriesRequest>
  implements OnInit, OnDestroy
{
  private categoriesApi = inject(VehicleCategoriesApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private destroy$ = new Subject<void>();

  displayedColumns = ['name', 'multiplier', 'actions'];
  searchCtrl = new FormControl('');

  constructor() {
    super();
    this.request = new ListVehicleCategoriesRequest();
  }

  ngOnInit(): void {
    this.searchCtrl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((val) => {
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
    this.categoriesApi
      .list(this.request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.handlePageResult(res);
          this.stopLoading();
        },
        error: () => this.stopLoading('Failed to load vehicle categories.'),
      });
  }

  openCreateDialog(): void {
    this.dialog
      .open(VehicleCategoryUpsertDialogComponent, {
        width: '520px',
        maxWidth: '95vw',
        data: { category: null } as VehicleCategoryUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Vehicle category created.');
          this.loadPagedData();
        }
      });
  }

  openEditDialog(category: ListVehicleCategoriesQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialog
      .open(VehicleCategoryUpsertDialogComponent, {
        width: '520px',
        maxWidth: '95vw',
        data: { category } as VehicleCategoryUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Vehicle category updated.');
          this.loadPagedData();
        }
      });
  }

  deleteCategory(category: ListVehicleCategoriesQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialogHelper.confirmDelete(category.name).subscribe((result) => {
      if (result?.button !== DialogButton.DELETE) return;
      this.categoriesApi
        .delete(category.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.toaster.success('Vehicle category deleted.');
            this.loadPagedData();
          },
          error: (err) => {
            const msg = err?.error?.message ?? 'Failed to delete vehicle category.';
            this.toaster.error(msg);
          },
        });
    });
  }
}
