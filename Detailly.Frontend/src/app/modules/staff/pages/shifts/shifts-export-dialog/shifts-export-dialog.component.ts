import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { EmployeeShiftsApiService } from '../../../../../api-services/employee-shifts/employee-shifts-api.service';
import { EmployeeWorkMode } from '../../../../../api-services/employee-shifts/employee-shifts-api.models';
import { ToasterService } from '../../../../../core/services/toaster.service';

export interface ShiftsExportDialogData {
  shopLocationId: number;
  locationName: string;
  workModeFilter: EmployeeWorkMode | null;
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

  EmployeeWorkMode = EmployeeWorkMode;

  startDate: Date | null = null;
  endDate: Date | null = null;
  workModeFilter: EmployeeWorkMode | null = this.data.workModeFilter;
  isExporting = false;

  get locationName(): string {
    return this.data.locationName;
  }

  get isValid(): boolean {
    return !!this.startDate && !!this.endDate && this.startDate <= this.endDate;
  }

  export(): void {
    if (!this.isValid || !this.startDate || !this.endDate) return;

    this.isExporting = true;
    const start = this.formatDate(this.startDate);
    const end = this.formatDate(this.endDate);

    this.shiftsApi
      .exportShiftsPdf(start, end, this.data.shopLocationId, this.workModeFilter)
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
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
