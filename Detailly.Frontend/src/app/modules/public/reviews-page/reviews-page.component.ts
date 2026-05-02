import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReviewsApiService } from '../../../api-services/reviews/reviews-api.service';
import { ServicePackagesApiService } from '../../../api-services/service-packages/service-packages-api.service';
import { ListReviewsQueryDto, ListReviewsRequest } from '../../../api-services/reviews/reviews-api.models';
import { ListServicePackagesQueryDto } from '../../../api-services/service-packages/service-packages-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';

@Component({
  selector: 'app-reviews-page',
  standalone: false,
  templateUrl: './reviews-page.component.html',
  styleUrl: './reviews-page.component.scss',
})
export class ReviewsPageComponent implements OnInit {
  private reviewsService = inject(ReviewsApiService);
  private packagesService = inject(ServicePackagesApiService);
  private route = inject(ActivatedRoute);
  readonly currentUserService = inject(CurrentUserService);

  reviews: ListReviewsQueryDto[] = [];
  packages: ListServicePackagesQueryDto[] = [];

  isLoadingReviews = false;
  isLoadingPackages = false;

  totalItems = 0;
  currentPage = 1;
  pageSize = 5;

  selectedServicePackageId: number | null = null;
  sort: 'newest' | 'oldest' = 'newest';

  // Computed from loaded reviews/packages
  get selectedPackage(): ListServicePackagesQueryDto | undefined {
    return this.packages.find((p) => p.id === this.selectedServicePackageId);
  }

  get overallAverageRating(): number {
    if (this.packages.length === 0) return 0;
    const rated = this.packages.filter((p) => p.reviewCount > 0);
    if (rated.length === 0) return 0;
    const totalWeighted = rated.reduce(
      (sum, p) => sum + (p.averageRating ?? 0) * p.reviewCount,
      0,
    );
    const totalCount = rated.reduce((sum, p) => sum + p.reviewCount, 0);
    return totalCount > 0 ? totalWeighted / totalCount : 0;
  }

  get totalReviewCount(): number {
    return this.packages.reduce((sum, p) => sum + p.reviewCount, 0);
  }

  get ratingDistribution(): { stars: number; count: number; percent: number }[] {
    // Build aggregate distribution across all packages
    // We don't have per-star breakdown from the backend, so we estimate from loaded reviews
    const dist = [5, 4, 3, 2, 1].map((stars) => {
      const count = this.reviews.filter((r) => r.rating === stars).length;
      return { stars, count, percent: this.totalItems > 0 ? (count / this.totalItems) * 100 : 0 };
    });
    return dist;
  }

  ngOnInit(): void {
    this.loadPackages();

    // Support pre-selecting a service package via query param
    this.route.queryParams.subscribe((params) => {
      const spId = params['servicePackageId'];
      if (spId) {
        this.selectedServicePackageId = Number(spId);
      }
      this.loadReviews();
    });
  }

  loadPackages(): void {
    this.isLoadingPackages = true;
    this.packagesService.list().subscribe({
      next: (result) => {
        this.packages = result.items;
        this.isLoadingPackages = false;
      },
      error: () => {
        this.isLoadingPackages = false;
      },
    });
  }

  loadReviews(): void {
    this.isLoadingReviews = true;
    const req = new ListReviewsRequest();
    req.paging.page = this.currentPage;
    req.paging.pageSize = this.pageSize;
    req.sort = this.sort;
    if (this.selectedServicePackageId) {
      req.servicePackageId = this.selectedServicePackageId;
    }

    this.reviewsService.list(req).subscribe({
      next: (result) => {
        this.reviews = result.items;
        this.totalItems = result.total;
        this.isLoadingReviews = false;
      },
      error: () => {
        this.isLoadingReviews = false;
      },
    });
  }

  onPackageFilterChange(): void {
    this.currentPage = 1;
    this.loadReviews();
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadReviews();
  }

  goToPage(page: number): void {
    this.currentPage = page;
    this.loadReviews();
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  getStarArray(rating: number): ('full' | 'empty')[] {
    return [1, 2, 3, 4, 5].map((i) => (i <= rating ? 'full' : 'empty'));
  }

  readonly Math = Math;

  formatRating(val: number | null | undefined): string {
    if (!val) return '—';
    return val.toFixed(1);
  }
}
