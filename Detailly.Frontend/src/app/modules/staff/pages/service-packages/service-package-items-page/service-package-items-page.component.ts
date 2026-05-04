import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { BaseListPagedComponent } from '../../../../../core/components/base-classes/base-list-paged-component';
import {
  ListServicePackageItemsQueryDto,
  ListServicePackageItemsRequest,
} from '../../../../../api-services/service-package-items/service-package-items-api.models';
import { ServicePackageItemsApiService } from '../../../../../api-services/service-package-items/service-package-items-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../../shared/models/dialog-config.model';
import {
  ServicePackageItemUpsertDialogComponent,
  ServicePackageItemUpsertDialogData,
} from './service-package-item-upsert-dialog/service-package-item-upsert-dialog.component';

@Component({
  selector: 'app-service-package-items-page',
  standalone: false,
  templateUrl: './service-package-items-page.component.html',
  styleUrl: './service-package-items-page.component.scss',
})
export class ServicePackageItemsPageComponent
  extends BaseListPagedComponent<ListServicePackageItemsQueryDto, ListServicePackageItemsRequest>
  implements OnInit, OnDestroy
{
  private itemsApi = inject(ServicePackageItemsApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  displayedColumns = ['name', 'type', 'duration', 'employees', 'price', 'status', 'actions'];
  searchCtrl = new FormControl('');

  constructor() {
    super();
    this.request = new ListServicePackageItemsRequest();
    this.request.paging.pageSize = 20;
    this.request.includeInactive = true;
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
    this.itemsApi
      .list(this.request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.handlePageResult(res);
          this.stopLoading();
        },
        error: () => this.stopLoading('Failed to load service package items.'),
      });
  }

  goBack(): void {
    this.router.navigate(['/staff/service-packages']);
  }

  openCreateDialog(): void {
    this.dialog
      .open(ServicePackageItemUpsertDialogComponent, {
        width: '600px',
        maxWidth: '95vw',
        data: { item: null } as ServicePackageItemUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Service package item created.');
          this.loadPagedData();
        }
      });
  }

  openEditDialog(item: ListServicePackageItemsQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialog
      .open(ServicePackageItemUpsertDialogComponent, {
        width: '600px',
        maxWidth: '95vw',
        data: { item } as ServicePackageItemUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Service package item updated.');
          this.loadPagedData();
        }
      });
  }

  deleteItem(item: ListServicePackageItemsQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialogHelper.confirmDelete(item.name).subscribe((result) => {
      if (result?.button !== DialogButton.DELETE) return;
      this.itemsApi
        .delete(item.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.toaster.success('Service package item deleted.');
            this.loadPagedData();
          },
          error: (err) => {
            const msg = err?.error?.message ?? 'Failed to delete service package item.';
            this.toaster.error(msg);
          },
        });
    });
  }
}
