import { Component, Input } from '@angular/core';
import { ListReviewsQueryDto } from '../../../../api-services/reviews/reviews-api.models';

@Component({
  selector: 'app-testimonials',
  templateUrl: './testimonials.component.html',
  styleUrl: './testimonials.component.scss',
  standalone: false,
})
export class TestimonialsComponent {
  @Input() reviews: ListReviewsQueryDto[] = [];

  currentReviewIndex = 0;

  get currentReview(): ListReviewsQueryDto | undefined {
    return this.reviews[this.currentReviewIndex];
  }

  nextReview(): void {
    this.currentReviewIndex = (this.currentReviewIndex + 1) % this.reviews.length;
  }

  prevReview(): void {
    this.currentReviewIndex =
      (this.currentReviewIndex - 1 + this.reviews.length) % this.reviews.length;
  }

  goToReview(index: number): void {
    this.currentReviewIndex = index;
  }

  getRatingArray(rating: number): number[] {
    return Array(5)
      .fill(0)
      .map((_, i) => i);
  }
}
