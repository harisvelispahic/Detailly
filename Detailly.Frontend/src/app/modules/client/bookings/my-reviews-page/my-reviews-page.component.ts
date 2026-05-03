import { Component, OnInit } from '@angular/core';
import { inject } from '@angular/core';
import { ReviewsApiService } from '../../../../api-services/reviews/reviews-api.service';
import { ListMyReviewsQueryDto, ListMyReviewsRequest } from '../../../../api-services/reviews/reviews-api.models';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';

@Component({
  selector: 'app-my-reviews-page',
  standalone: false,
  templateUrl: './my-reviews-page.component.html',
  styleUrl: './my-reviews-page.component.scss',
})
export class MyReviewsPageComponent
  extends BaseListPagedComponent<ListMyReviewsQueryDto, ListMyReviewsRequest>
  implements OnInit
{
  private reviewsService = inject(ReviewsApiService);

  sort: 'newest' | 'oldest' = 'newest';

  constructor() {
    super();
    this.request = new ListMyReviewsRequest();
  }

  ngOnInit(): void {
    this.initList();
  }

  protected loadPagedData(): void {
    this.startLoading();
    this.request.sort = this.sort;

    this.reviewsService.listMy(this.request).subscribe({
      next: (result) => {
        this.handlePageResult(result);
        this.stopLoading();
      },
      error: () => this.stopLoading(),
    });
  }

  onSortChange(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  getStarArray(rating: number): ('full' | 'empty')[] {
    return [1, 2, 3, 4, 5].map((i) => (i <= rating ? 'full' : 'empty'));
  }
}
