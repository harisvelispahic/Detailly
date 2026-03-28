import { NgModule } from '@angular/core';

import { PublicRoutingModule } from './public-routing-module';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { SearchProductsComponent } from './search-products/search-products.component';
import { SharedModule } from '../shared/shared-module';
import { LandingComponent } from './landing/landing.component';
import { HeroComponent } from './landing/hero/hero.component';
import { FeaturesComponent } from './landing/features/features.component';
import { ServicesComponent } from './landing/services/services.component';
import { PricingComponent } from './landing/pricing/pricing.component';
import { TestimonialsComponent } from './landing/testimonials/testimonials.component';
import { FinalCtaComponent } from './landing/final-cta/final-cta.component';

@NgModule({
  declarations: [
    PublicLayoutComponent,
    SearchProductsComponent,
    LandingComponent,
    HeroComponent,
    FeaturesComponent,
    ServicesComponent,
    PricingComponent,
    TestimonialsComponent,
    FinalCtaComponent,
  ],
  imports: [SharedModule, PublicRoutingModule],
})
export class PublicModule {}
