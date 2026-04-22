import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddressesApiService } from '../../../../api-services/addresses/addresses-api.service';
import { ListMyAddressesQueryDto } from '../../../../api-services/addresses/addresses-api.model';

export interface AddressDialogData {
  address?: ListMyAddressesQueryDto;
}

@Component({
  selector: 'app-address-dialog',
  standalone: false,
  templateUrl: './address-dialog.component.html',
  styleUrl: './address-dialog.component.scss',
})
export class AddressDialogComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  error?: string;

  get isEdit(): boolean {
    return !!this.data.address;
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddressDialogComponent>,
    private addresses: AddressesApiService,
    @Inject(MAT_DIALOG_DATA) public data: AddressDialogData,
  ) {}

  ngOnInit(): void {
    const a = this.data.address;
    this.form = this.fb.group({
      street: [a?.street ?? '', Validators.required],
      city: [a?.city ?? '', Validators.required],
      postalCode: [a?.postalCode ?? '', Validators.required],
      region: [a?.region ?? ''],
      country: [a?.country ?? '', Validators.required],
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.error = undefined;
    this.isLoading = true;
    const value = this.form.value;

    if (this.isEdit) {
      this.addresses.update(this.data.address!.id, value).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.error = err?.error?.message ?? 'Update failed.';
          this.isLoading = false;
        },
      });
    } else {
      this.addresses.create(value).subscribe({
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
