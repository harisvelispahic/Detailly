import { Component, inject, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CreateServicePackageItemCommand,
  ListServicePackageItemsQueryDto,
  UpdateServicePackageItemCommand,
} from '../../../../../../api-services/service-package-items/service-package-items-api.models';
import { ServicePackageItemsApiService } from '../../../../../../api-services/service-package-items/service-package-items-api.service';
import { extractHttpError } from '../../../../../../core/utils/http-error.util';

export interface ServicePackageItemUpsertDialogData {
  item: ListServicePackageItemsQueryDto | null;
}

@Component({
  selector: 'app-service-package-item-upsert-dialog',
  standalone: false,
  templateUrl: './service-package-item-upsert-dialog.component.html',
  styleUrl: './service-package-item-upsert-dialog.component.scss',
})
export class ServicePackageItemUpsertDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private itemsApi = inject(ServicePackageItemsApiService);
  private dialogRef = inject(MatDialogRef<ServicePackageItemUpsertDialogComponent>);

  form!: FormGroup;
  isSubmitting = false;
  serverError: string | null = null;

  get isEditMode(): boolean {
    return this.data.item !== null;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: ServicePackageItemUpsertDialogData) {}

  ngOnInit(): void {
    const i = this.data.item;

    this.form = this.fb.group({
      name: [i?.name ?? '', [Validators.required, Validators.maxLength(200)]],
      description: [i?.description ?? '', [Validators.maxLength(2000)]],
      price: [i?.price ?? null, [Validators.required, Validators.min(0)]],
      durationMinutes: [i?.durationMinutes ?? null, [Validators.required, Validators.min(1)]],
      requiredEmployees: [i?.requiredEmployees ?? 1, [Validators.required, Validators.min(1)]],
      isAddon: [i?.isAddon ?? false],
      isActive: [i?.isActive ?? true],
    });
  }

  onSave(): void {
    if (this.form.invalid || this.isSubmitting) return;
    this.isSubmitting = true;
    this.serverError = null;

    const v = this.form.getRawValue();

    if (this.isEditMode) {
      const command: UpdateServicePackageItemCommand = {
        name: v.name.trim(),
        description: v.description?.trim() || null,
        price: v.price,
        durationMinutes: v.durationMinutes,
        requiredEmployees: v.requiredEmployees,
        isAddon: v.isAddon,
        isActive: v.isActive,
      };
      this.itemsApi.update(this.data.item!.id, command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.serverError = extractHttpError(err) ?? 'Failed to update service package item.';
        },
      });
    } else {
      const command: CreateServicePackageItemCommand = {
        name: v.name.trim(),
        description: v.description?.trim() || null,
        price: v.price,
        durationMinutes: v.durationMinutes,
        requiredEmployees: v.requiredEmployees,
        isAddon: v.isAddon,
      };
      this.itemsApi.create(command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.serverError = extractHttpError(err) ?? 'Failed to create service package item.';
        },
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

}
