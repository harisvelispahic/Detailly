import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { VehiclesApiService } from '../../../../api-services/vehicles/vehicles-api.service';
import { VehicleCategoriesApiService } from '../../../../api-services/vehicle-categories/vehicle-categories-api.service';
import { VehicleCategoryDto } from '../../../../api-services/vehicle-categories/vehicle-categories-api.model';
import { ListMyVehiclesQueryDto } from '../../../../api-services/vehicles/vehicles-api.model';

export interface VehicleDialogData {
  vehicle?: ListMyVehiclesQueryDto;
}

@Component({
  selector: 'app-vehicle-dialog',
  standalone: false,
  templateUrl: './vehicle-dialog.component.html',
  styleUrl: './vehicle-dialog.component.scss',
})
export class VehicleDialogComponent implements OnInit {
  form!: FormGroup;
  categories: VehicleCategoryDto[] = [];
  isLoading = false;
  isLoadingCategories = false;
  error?: string;

  get isEdit(): boolean {
    return !!this.data.vehicle;
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<VehicleDialogComponent>,
    private vehicles: VehiclesApiService,
    private vehicleCategories: VehicleCategoriesApiService,
    @Inject(MAT_DIALOG_DATA) public data: VehicleDialogData,
  ) {}

  ngOnInit(): void {
    const v = this.data.vehicle;
    this.form = this.fb.group({
      brand: [v?.brand ?? '', Validators.required],
      model: [v?.model ?? '', Validators.required],
      yearOfManufacture: [v?.yearOfManufacture ?? new Date().getFullYear(), [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
      vehicleCategoryId: [null, Validators.required],
      licencePlate: [v?.licencePlate ?? '', Validators.required],
      notes: [v?.notes ?? ''],
    });

    this.loadCategories(v?.vehicleCategory?.name);
  }

  private loadCategories(currentCategoryName?: string): void {
    this.isLoadingCategories = true;
    this.vehicleCategories.list().subscribe({
      next: (cats) => {
        this.categories = cats;
        if (currentCategoryName) {
          const match = cats.find(c => c.name === currentCategoryName);
          if (match) {
            this.form.patchValue({ vehicleCategoryId: match.id });
          }
        }
        this.isLoadingCategories = false;
      },
      error: () => {
        this.isLoadingCategories = false;
      },
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.error = undefined;
    this.isLoading = true;
    const value = this.form.value;

    if (this.isEdit) {
      this.vehicles.update(this.data.vehicle!.id, value).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.error = err?.error?.message ?? 'Update failed.';
          this.isLoading = false;
        },
      });
    } else {
      this.vehicles.create(value).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.error = err?.error?.message ?? 'Create failed.';
          this.isLoading = false;
        },
      });
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
