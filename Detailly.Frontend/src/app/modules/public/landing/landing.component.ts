import { Component, OnInit } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { ServicePackagesApiService } from '../../../api-services/service-packages/service-packages-api.service';
import { ReviewsApiService } from '../../../api-services/reviews/reviews-api.service';
import {
  ListServicePackagesRequest,
} from '../../../api-services/service-packages/service-packages-api.models';
import { ListReviewsQueryDto, ListReviewsRequest } from '../../../api-services/reviews/reviews-api.models';
import { Feature } from './features/features.component';
import { LandingStats } from './hero/hero.component';
import { ServicePackageWithAddons } from './services/services.component';

@Component({
  selector: 'app-detailing-landing',
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
  standalone: false,
})
export class LandingComponent implements OnInit {
  stats: LandingStats = {
    carsDetailed: '5,000+',
    averageRating: '4.9',
    yearsExperience: '10+',
  };

  features: Feature[] = [
    {
      icon: 'sparkles',
      title: 'Premium Products',
      description: 'We use only the highest quality detailing products for your vehicle.',
    },
    {
      icon: 'shield',
      title: 'Paint Protection',
      description: 'Advanced ceramic coatings and sealants for long-lasting protection.',
    },
    {
      icon: 'schedule',
      title: 'Convenient Booking',
      description: 'Easy online scheduling with flexible time slots that fit your life.',
    },
    {
      icon: 'verified_user',
      title: 'Expert Team',
      description: 'Certified detailers with years of experience in vehicle care.',
    },
  ];

  services: ServicePackageWithAddons[] = [];
  reviews: ListReviewsQueryDto[] = [];

  constructor(
    private servicePackagesApi: ServicePackagesApiService,
    private reviewsApi: ReviewsApiService
  ) {}

  ngOnInit(): void {
    const servicesReq = new ListServicePackagesRequest();
    servicesReq.paging.pageSize = 3;

    this.servicePackagesApi
      .list(servicesReq)
      .pipe(
        switchMap(result => {
          const pkgs = result.items;
          if (pkgs.length === 0) return of([] as ServicePackageWithAddons[]);
          return forkJoin(
            pkgs.map(pkg =>
              this.servicePackagesApi
                .getAvailableAddons(pkg.id)
                .pipe(map(addons => ({ ...pkg, availableAddons: addons.items })))
            )
          );
        })
      )
      .subscribe(packages => {
        this.services = packages;
      });

    const reviewsReq = new ListReviewsRequest();
    reviewsReq.paging.pageSize = 5;
    this.reviewsApi.list(reviewsReq).subscribe(result => {
      this.reviews = result.items;
    });

    this.reviewsApi.getStats().subscribe(stats => {
      if (stats.totalCount > 0) {
        this.stats = { ...this.stats, averageRating: stats.averageRating.toFixed(1) };
      }
    });
  }
}
