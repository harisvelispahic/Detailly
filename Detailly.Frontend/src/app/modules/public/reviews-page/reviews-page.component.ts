import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReviewsApiService } from '../../../api-services/reviews/reviews-api.service';
import { ServicePackagesApiService } from '../../../api-services/service-packages/service-packages-api.service';
import { ListReviewsQueryDto, ListReviewsRequest } from '../../../api-services/reviews/reviews-api.models';
import { ListServicePackagesQueryDto } from '../../../api-services/service-packages/service-packages-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';

@Component({
  selector: 'app-reviews-page',
  standalone: false,
  templateUrl: './reviews-page.component.html',
  styleUrl: './reviews-page.component.scss',
})
export class ReviewsPageComponent
  extends BaseListPagedComponent<ListReviewsQueryDto, ListReviewsRequest>
  implements OnInit
{
  private reviewsService = inject(ReviewsApiService);
  private packagesService = inject(ServicePackagesApiService);
  private route = inject(ActivatedRoute);
  readonly currentUserService = inject(CurrentUserService);

  packages: ListServicePackagesQueryDto[] = [];
  isLoadingPackages = false;

  selectedServicePackageId: number | null = null;
  sort: 'newest' | 'oldest' = 'newest';

  constructor() {
    super();
    this.request = new ListReviewsRequest();
  }

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
    return [5, 4, 3, 2, 1].map((stars) => {
      const count = this.items.filter((r) => r.rating === stars).length;
      return { stars, count, percent: this.totalItems > 0 ? (count / this.totalItems) * 100 : 0 };
    });
  }

  ngOnInit(): void {
    this.loadPackages();
    this.route.queryParams.subscribe((params) => {
      const spId = params['servicePackageId'];
      if (spId) {
        this.selectedServicePackageId = Number(spId);
      }
      this.request.paging.page = 1;
      this.loadPagedData();
    });
  }

  protected loadPagedData(): void {
    this.startLoading();
    this.request.sort = this.sort;
    this.request.servicePackageId = this.selectedServicePackageId ?? undefined;

    this.reviewsService.list(this.request).subscribe({
      next: (result) => {
        this.handlePageResult(result);
        this.stopLoading();
      },
      error: () => this.stopLoading(),
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

  onPackageFilterChange(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onSortChange(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
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
