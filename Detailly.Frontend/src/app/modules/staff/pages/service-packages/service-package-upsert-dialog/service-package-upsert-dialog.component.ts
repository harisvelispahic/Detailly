import { Component, inject, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CreateServicePackageCommand,
  ListServicePackagesQueryDto,
  UpdateServicePackageCommand,
} from '../../../../../api-services/service-packages/service-packages-api.models';
import { ServicePackagesApiService } from '../../../../../api-services/service-packages/service-packages-api.service';
import { ServicePackageItemsApiService } from '../../../../../api-services/service-package-items/service-package-items-api.service';
import { ListServicePackageItemsQueryDto } from '../../../../../api-services/service-package-items/service-package-items-api.models';
import { extractHttpError } from '../../../../../core/utils/http-error.util';

export interface ServicePackageUpsertDialogData {
  servicePackage: ListServicePackagesQueryDto | null;
}

@Component({
  selector: 'app-service-package-upsert-dialog',
  standalone: false,
  templateUrl: './service-package-upsert-dialog.component.html',
  styleUrl: './service-package-upsert-dialog.component.scss',
})
export class ServicePackageUpsertDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private packagesApi = inject(ServicePackagesApiService);
  private itemsApi = inject(ServicePackageItemsApiService);
  private dialogRef = inject(MatDialogRef<ServicePackageUpsertDialogComponent>);

  form!: FormGroup;
  isSubmitting = false;
  serverError: string | null = null;

  allItems: ListServicePackageItemsQueryDto[] = [];
  filteredItems: ListServicePackageItemsQueryDto[] = [];
  selectedItemIds = new Set<number>();
  itemSearchCtrl = new FormControl('');
  isLoadingItems = false;

  get isEditMode(): boolean {
    return this.data.servicePackage !== null;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: ServicePackageUpsertDialogData) {}

  ngOnInit(): void {
    const pkg = this.data.servicePackage;

    this.form = this.fb.group({
      name: [pkg?.name ?? '', [Validators.required, Validators.maxLength(200)]],
      description: [pkg?.description ?? '', [Validators.maxLength(2000)]],
      price: [pkg?.price ?? null, [Validators.required, Validators.min(0)]],
    });

    if (this.isEditMode && pkg?.items?.length) {
      pkg.items.forEach((i) => this.selectedItemIds.add(i.id));
    }

    this.itemSearchCtrl.valueChanges.subscribe((val) => {
      this.applyItemFilter(val ?? '');
    });

    this.loadItems();
  }

  private loadItems(): void {
    this.isLoadingItems = true;
    this.itemsApi.list().subscribe({
      next: (res) => {
        this.allItems = res.items as ListServicePackageItemsQueryDto[];
        this.applyItemFilter('');
        this.isLoadingItems = false;
      },
      error: () => {
        this.isLoadingItems = false;
      },
    });
  }

  private applyItemFilter(search: string): void {
    const term = search.trim().toLowerCase();
    this.filteredItems = term
      ? this.allItems.filter(
          (i) =>
            i.name.toLowerCase().includes(term) ||
            (i.description ?? '').toLowerCase().includes(term),
        )
      : [...this.allItems];
  }

  toggleItem(id: number): void {
    if (this.selectedItemIds.has(id)) {
      this.selectedItemIds.delete(id);
    } else {
      this.selectedItemIds.add(id);
    }
  }

  isItemSelected(id: number): boolean {
    return this.selectedItemIds.has(id);
  }

  get selectedCount(): number {
    return this.selectedItemIds.size;
  }

  onSave(): void {
    if (this.form.invalid || this.isSubmitting) return;
    this.isSubmitting = true;
    this.serverError = null;

    const v = this.form.getRawValue();
    const itemIds = Array.from(this.selectedItemIds);

    if (this.isEditMode) {
      const command: UpdateServicePackageCommand = {
        name: v.name.trim(),
        description: v.description?.trim() || null,
        price: v.price,
        servicePackageItemIds: itemIds,
      };
      this.packagesApi.update(this.data.servicePackage!.id, command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.serverError = extractHttpError(err) ?? 'Failed to update service package.';
        },
      });
    } else {
      const command: CreateServicePackageCommand = {
        name: v.name.trim(),
        description: v.description?.trim() || null,
        price: v.price,
        servicePackageItemIds: itemIds,
      };
      this.packagesApi.create(command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.serverError = extractHttpError(err) ?? 'Failed to create service package.';
        },
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

}
