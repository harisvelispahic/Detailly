import { Component, OnInit } from '@angular/core';
import {
  mockLandingStats,
  mockFeatures,
  mockServices,
  mockPricingTiers,
  mockReviews,
} from '../../../data/landing-detailing.mock';
import {
  Feature,
  Service,
  PricingTier,
  Review,
  LandingStats,
} from '../../../data/landing-detailing.model';

@Component({
  selector: 'app-detailing-landing',
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
  standalone: false,
})
export class LandingComponent implements OnInit {
  stats: LandingStats = mockLandingStats;
  features: Feature[] = mockFeatures;
  services: Service[] = mockServices;
  pricingTiers: PricingTier[] = mockPricingTiers;
  reviews: Review[] = mockReviews;

  constructor() {}

  ngOnInit(): void {
    // TODO: Load from API service when backend is ready
    // this.landingService.getLandingData().subscribe(data => {
    //   this.stats = data.stats;
    //   this.features = data.features;
    //   this.services = data.services;
    //   this.pricingTiers = data.pricingTiers;
    //   this.reviews = data.reviews;
    // });
  }
}
