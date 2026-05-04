import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import {
  ListServicePackagesQueryDto,
  ListServicePackagesRequest,
} from '../../../../api-services/service-packages/service-packages-api.models';
import { ServicePackagesApiService } from '../../../../api-services/service-packages/service-packages-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import {
  ServicePackageUpsertDialogComponent,
  ServicePackageUpsertDialogData,
} from './service-package-upsert-dialog/service-package-upsert-dialog.component';

@Component({
  selector: 'app-service-packages',
  standalone: false,
  templateUrl: './service-packages.component.html',
  styleUrl: './service-packages.component.scss',
})
export class ServicePackagesComponent
  extends BaseListPagedComponent<ListServicePackagesQueryDto, ListServicePackagesRequest>
  implements OnInit, OnDestroy
{
  private packagesApi = inject(ServicePackagesApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  displayedColumns = ['name', 'price', 'items', 'actions'];
  searchCtrl = new FormControl('');

  constructor() {
    super();
    this.request = new ListServicePackagesRequest();
    this.request.paging.pageSize = 20;
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
    this.packagesApi
      .list(this.request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.handlePageResult(res);
          this.stopLoading();
        },
        error: () => this.stopLoading('Failed to load service packages.'),
      });
  }

  goToItems(): void {
    this.router.navigate(['/staff/service-packages/items']);
  }

  openCreateDialog(): void {
    this.dialog
      .open(ServicePackageUpsertDialogComponent, {
        width: '680px',
        maxWidth: '95vw',
        data: { servicePackage: null } as ServicePackageUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Service package created.');
          this.loadPagedData();
        }
      });
  }

  openEditDialog(pkg: ListServicePackagesQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialog
      .open(ServicePackageUpsertDialogComponent, {
        width: '680px',
        maxWidth: '95vw',
        data: { servicePackage: pkg } as ServicePackageUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Service package updated.');
          this.loadPagedData();
        }
      });
  }

  deletePackage(pkg: ListServicePackagesQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialogHelper.confirmDelete(pkg.name).subscribe((result) => {
      if (result?.button !== DialogButton.DELETE) return;
      this.packagesApi
        .delete(pkg.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.toaster.success('Service package deleted.');
            this.loadPagedData();
          },
          error: (err) => {
            const msg = err?.error?.message ?? 'Failed to delete service package.';
            this.toaster.error(msg);
          },
        });
    });
  }

  getItemsSummary(pkg: ListServicePackagesQueryDto): string {
    if (!pkg.items?.length) return 'No items';
    return pkg.items.map((i) => i.name).join(', ');
  }
}
