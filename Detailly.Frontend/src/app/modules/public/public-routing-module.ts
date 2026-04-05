import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SearchProductsComponent } from './search-products/search-products.component';
import { LandingComponent } from './landing/landing.component';
import { PublicPlaceholderPageComponent } from './placeholder-page/public-placeholder-page.component';
import { PublicLayoutComponent } from './public-layout/public-layout.component';

const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: '',
        component: LandingComponent,
      },
      {
        path: 'search',
        component: SearchProductsComponent,
      },
      {
        path: 'shop',
        redirectTo: 'search',
        pathMatch: 'full',
      },
      {
        path: 'booking',
        redirectTo: 'book-now',
        pathMatch: 'full',
      },
      {
        path: 'book-now',
        component: PublicPlaceholderPageComponent,
        data: {
          eyebrow: 'Booking',
          title: 'Book Your Next',
          accent: 'Detail',
          description:
            'The full multi-step booking experience is still to be implemented, but this route now exists so the new navbar and footer behave like the React shell.',
          highlights: [
            'Service and add-on selection',
            'Vehicle, address, and schedule flow',
            'Confirmation and checkout handoff',
          ],
          primaryLabel: 'Browse Products',
          primaryRoute: '/search',
          secondaryLabel: 'Read Reviews',
          secondaryRoute: '/reviews',
        },
      },
      {
        path: 'reviews',
        component: PublicPlaceholderPageComponent,
        data: {
          eyebrow: 'Social Proof',
          title: 'Customer',
          accent: 'Reviews',
          description:
            'This page is reserved for the full testimonials and rating experience. For now it keeps the global navigation complete and consistent.',
          highlights: [
            'Rating summary and distribution',
            'Review cards and sorting',
            'Future write-a-review flow',
          ],
          primaryLabel: 'Book Now',
          primaryRoute: '/book-now',
          secondaryLabel: 'Back Home',
          secondaryRoute: '/',
        },
      },
      {
        path: 'privacy-policy',
        component: PublicPlaceholderPageComponent,
        data: {
          eyebrow: 'Legal',
          title: 'Privacy',
          accent: 'Policy',
          description:
            'A dedicated legal page now exists here. The full privacy copy can be filled in during the next content pass.',
          highlights: [
            'Data collection summary',
            'Processing and retention details',
            'Customer rights and contact information',
          ],
          primaryLabel: 'Return Home',
          primaryRoute: '/',
          secondaryLabel: 'Terms of Service',
          secondaryRoute: '/terms-of-service',
        },
      },
      {
        path: 'terms-of-service',
        component: PublicPlaceholderPageComponent,
        data: {
          eyebrow: 'Legal',
          title: 'Terms of',
          accent: 'Service',
          description:
            'This placeholder keeps the footer legal navigation functional until the full terms content is added.',
          highlights: [
            'Booking and payment terms',
            'Service limitations and cancellations',
            'Support and dispute handling',
          ],
          primaryLabel: 'Return Home',
          primaryRoute: '/',
          secondaryLabel: 'Privacy Policy',
          secondaryRoute: '/privacy-policy',
        },
      },
    ],
  },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PublicRoutingModule {}
