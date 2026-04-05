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
  ],
  imports: [SharedModule, PublicRoutingModule],
})
export class PublicModule {}
