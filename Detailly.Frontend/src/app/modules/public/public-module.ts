import { NgModule } from '@angular/core';

import { PublicRoutingModule } from './public-routing-module';
import { SearchProductsComponent } from './search-products/search-products.component';
import { SharedModule } from '../shared/shared-module';
import { LandingComponent } from './landing/landing.component';
import { HeroComponent } from './landing/hero/hero.component';
import { FeaturesComponent } from './landing/features/features.component';
import { ServicesComponent } from './landing/services/services.component';
import { PricingComponent } from './landing/pricing/pricing.component';
import { TestimonialsComponent } from './landing/testimonials/testimonials.component';
import { FinalCtaComponent } from './landing/final-cta/final-cta.component';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { PublicPlaceholderPageComponent } from './placeholder-page/public-placeholder-page.component';
import { BookingWizardComponent } from './booking-wizard/booking-wizard.component';
import { ReviewsPageComponent } from './reviews-page/reviews-page.component';
import { ServicePackageDetailsDialogComponent } from './booking-wizard/service-package-details-dialog/service-package-details-dialog.component';

@NgModule({
  declarations: [
    SearchProductsComponent,
    LandingComponent,
    HeroComponent,
    FeaturesComponent,
    ServicesComponent,
    PricingComponent,
    TestimonialsComponent,
    FinalCtaComponent,
    PublicLayoutComponent,
    PublicPlaceholderPageComponent,
    BookingWizardComponent,
    ReviewsPageComponent,
    ServicePackageDetailsDialogComponent,
  ],
  imports: [SharedModule, PublicRoutingModule],
})
export class PublicModule {}
