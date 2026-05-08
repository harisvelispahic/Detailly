import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { EmployeeShiftsApiService } from '../../../../../api-services/employee-shifts/employee-shifts-api.service';
import { EmployeeWorkMode } from '../../../../../api-services/employee-shifts/employee-shifts-api.models';
import { ToasterService } from '../../../../../core/services/toaster.service';

export interface ShiftsExportDialogData {
  shopLocationId: number;
  locationName: string;
  workModeFilter: EmployeeWorkMode | null;
}

function endAfterStartValidator(control: AbstractControl): ValidationErrors | null {
  const start = control.get('startDate')?.value as Date | null;
  const end = control.get('endDate')?.value as Date | null;
  if (start && end && start > end) {
    return { endBeforeStart: true };
  }
  return null;
}

@Component({
  selector: 'app-shifts-export-dialog',
  standalone: false,
  templateUrl: './shifts-export-dialog.component.html',
  styleUrl: './shifts-export-dialog.component.scss',
})
export class ShiftsExportDialogComponent {
  private dialogRef = inject(MatDialogRef<ShiftsExportDialogComponent>);
  private data: ShiftsExportDialogData = inject(MAT_DIALOG_DATA);
  private shiftsApi = inject(EmployeeShiftsApiService);
  private toaster = inject(ToasterService);
  private fb = inject(FormBuilder);

  EmployeeWorkMode = EmployeeWorkMode;
  isExporting = false;

  form = this.fb.group(
    {
      startDate: [null as Date | null, Validators.required],
      endDate: [null as Date | null, Validators.required],
      workModeFilter: [this.data.workModeFilter as EmployeeWorkMode | null],
    },
    { validators: endAfterStartValidator },
  );

  get locationName(): string {
    return this.data.locationName;
  }

  export(): void {
    if (this.form.invalid || this.isExporting) return;

    const { startDate, endDate, workModeFilter } = this.form.value;
    if (!startDate || !endDate) return;

    this.isExporting = true;
    const start = this.formatDate(startDate);
    const end = this.formatDate(endDate);

    this.shiftsApi
      .exportShiftsPdf(start, end, this.data.shopLocationId, workModeFilter ?? null)
      .subscribe({
        next: (blob) => {
          this.downloadBlob(blob, `shifts-${start}-to-${end}.pdf`);
          this.isExporting = false;
          this.dialogRef.close();
        },
        error: () => {
          this.toaster.error('Failed to generate PDF report.');
          this.isExporting = false;
        },
      });
  }

  cancel(): void {
    this.dialogRef.close();
  }

  private formatDate(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  private downloadBlob(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    setTimeout(() => window.URL.revokeObjectURL(url), 150);
  }
}
