import { Component, inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { BookingsService } from '../../../../../api-services/bookings/bookings-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';

export interface BookingsExportDialogResult {
  exported: boolean;
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
  selector: 'app-bookings-export-dialog',
  standalone: false,
  templateUrl: './bookings-export-dialog.component.html',
  styleUrl: './bookings-export-dialog.component.scss',
})
export class BookingsExportDialogComponent {
  private dialogRef = inject(MatDialogRef<BookingsExportDialogComponent>);
  private bookingsService = inject(BookingsService);
  private toaster = inject(ToasterService);
  private fb = inject(FormBuilder);

  isExporting = false;

  form = this.fb.group(
    {
      startDate: [null as Date | null, Validators.required],
      endDate: [null as Date | null, Validators.required],
    },
    { validators: endAfterStartValidator },
  );

  get startDateCtrl() { return this.form.controls.startDate; }
  get endDateCtrl() { return this.form.controls.endDate; }

  export(): void {
    if (this.form.invalid || this.isExporting) return;

    const { startDate, endDate } = this.form.value;
    if (!startDate || !endDate) return;

    this.isExporting = true;
    const start = this.formatDate(startDate);
    const end = this.formatDate(endDate);

    this.bookingsService.exportMyBookingsPdf(start, end).subscribe({
      next: (blob) => {
        this.downloadBlob(blob, `my-appointments-${start}-to-${end}.pdf`);
        this.isExporting = false;
        this.dialogRef.close({ exported: true });
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
