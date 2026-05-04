import { Component, inject, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CreateVehicleCategoryCommand,
  ListVehicleCategoriesQueryDto,
  UpdateVehicleCategoryCommand,
} from '../../../../../api-services/vehicle-categories/vehicle-categories-api.model';
import { VehicleCategoriesApiService } from '../../../../../api-services/vehicle-categories/vehicle-categories-api.service';

export interface VehicleCategoryUpsertDialogData {
  category: ListVehicleCategoriesQueryDto | null;
}

@Component({
  selector: 'app-vehicle-category-upsert-dialog',
  standalone: false,
  templateUrl: './vehicle-category-upsert-dialog.component.html',
  styleUrl: './vehicle-category-upsert-dialog.component.scss',
})
export class VehicleCategoryUpsertDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private categoriesApi = inject(VehicleCategoriesApiService);
  private dialogRef = inject(MatDialogRef<VehicleCategoryUpsertDialogComponent>);

  form!: FormGroup;
  isSubmitting = false;
  serverError: string | null = null;

  get isEditMode(): boolean {
    return this.data.category !== null;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: VehicleCategoryUpsertDialogData) {}

  ngOnInit(): void {
    const cat = this.data.category;

    this.form = this.fb.group({
      name: [cat?.name ?? '', [Validators.required, Validators.maxLength(200)]],
      description: [cat?.description ?? '', [Validators.maxLength(2000)]],
      basePriceMultiplier: [
        cat?.basePriceMultiplier ?? null,
        [Validators.required, Validators.min(0.0001)],
      ],
    });
  }

  onSave(): void {
    if (this.form.invalid || this.isSubmitting) return;
    this.isSubmitting = true;
    this.serverError = null;

    const v = this.form.getRawValue();

    if (this.isEditMode) {
      const command: UpdateVehicleCategoryCommand = {
        name: v.name.trim(),
        description: v.description?.trim() || null,
        basePriceMultiplier: v.basePriceMultiplier,
      };
      this.categoriesApi.update(this.data.category!.id, command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.serverError = this.extractError(err) ?? 'Failed to update vehicle category.';
        },
      });
    } else {
      const command: CreateVehicleCategoryCommand = {
        name: v.name.trim(),
        description: v.description?.trim() || null,
        basePriceMultiplier: v.basePriceMultiplier,
      };
      this.categoriesApi.create(command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.serverError = this.extractError(err) ?? 'Failed to create vehicle category.';
        },
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private extractError(err: any): string | null {
    return err?.error?.message ?? err?.error?.title ?? err?.message ?? null;
  }
}
