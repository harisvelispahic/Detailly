import { Component, inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReviewsApiService } from '../../../../api-services/reviews/reviews-api.service';

export interface RateBookingDialogData {
  bookingId: number;
  servicePackageId: number;
  servicePackageName: string;
}

export interface RateBookingDialogResult {
  submitted: boolean;
}

@Component({
  selector: 'app-rate-booking-dialog',
  standalone: false,
  templateUrl: './rate-booking-dialog.component.html',
  styleUrl: './rate-booking-dialog.component.scss',
})
export class RateBookingDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<RateBookingDialogComponent>);
  private reviewsService = inject(ReviewsApiService);
  private fb = inject(FormBuilder);

  readonly data: RateBookingDialogData = inject(MAT_DIALOG_DATA);

  form!: FormGroup;
  isSubmitting = false;
  isLoading = true;
  errorMessage?: string;
  hoverRating = 0;
  isEditMode = false;

  readonly starLabels = ['', 'Poor', 'Fair', 'Good', 'Very Good', 'Excellent'];

  ngOnInit(): void {
    this.form = this.fb.group({
      rating: [0, [Validators.required, Validators.min(1), Validators.max(5)]],
      description: ['', Validators.maxLength(1000)],
    });

    this.reviewsService.getMyReviewForServicePackage(this.data.servicePackageId).subscribe({
      next: (existing) => {
        if (existing) {
          this.isEditMode = true;
          this.form.patchValue({ rating: existing.rating, description: existing.description ?? '' });
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  get selectedRating(): number {
    return this.form.get('rating')!.value as number;
  }

  setRating(stars: number): void {
    this.form.get('rating')!.setValue(stars);
  }

  get activeRating(): number {
    return this.hoverRating || this.selectedRating;
  }

  get descriptionLength(): number {
    return (this.form.get('description')!.value as string)?.length ?? 0;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isSubmitting = true;
    this.errorMessage = undefined;

    const { rating, description } = this.form.value as { rating: number; description: string };

    this.reviewsService
      .createOrUpdate(this.data.bookingId, { rating, description: description || null })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.dialogRef.close({ submitted: true } satisfies RateBookingDialogResult);
        },
        error: (err) => {
          this.isSubmitting = false;
          this.errorMessage = err?.error?.message ?? 'Failed to submit review. Please try again.';
        },
      });
  }

  cancel(): void {
    this.dialogRef.close({ submitted: false });
  }
}
