import { Component, inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { BookingsService } from '../../../../../api-services/bookings/bookings-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';

export interface BookingsExportDialogResult {
  exported: boolean;
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

  startDate: Date | null = null;
  endDate: Date | null = null;
  isExporting = false;

  get isValid(): boolean {
    return !!this.startDate && !!this.endDate && this.startDate <= this.endDate;
  }

  export(): void {
    if (!this.isValid || !this.startDate || !this.endDate) return;

    this.isExporting = true;
    const start = this.formatDate(this.startDate);
    const end = this.formatDate(this.endDate);

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
