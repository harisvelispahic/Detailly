import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SearchProductsComponent } from './search-products/search-products.component';
import { LandingComponent } from './landing/landing.component';
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
    ],
  },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PublicRoutingModule {}
