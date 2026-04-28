import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DashboardLayoutComponent } from '../shared/components/dashboard-layout/dashboard-layout.component';
import { ProductsComponent } from './catalogs/products/products.component';
import { ProductsAddComponent } from './catalogs/products/products-add/products-add.component';
import { ProductsEditComponent } from './catalogs/products/products-edit/products-edit.component';
import { ProductCategoriesComponent } from './catalogs/product-categories/product-categories.component';
import { AdminOrdersComponent } from './orders/admin-orders.component';
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';

const routes: Routes = [
  {
    path: '',
    component: DashboardLayoutComponent,
    children: [
      { path: 'products', component: ProductsComponent },
      { path: 'products/add', component: ProductsAddComponent },
      { path: 'products/:id/edit', component: ProductsEditComponent },
      { path: 'product-categories', component: ProductCategoriesComponent },
      { path: 'orders', component: AdminOrdersComponent },
      { path: 'settings', component: AdminSettingsComponent },
      { path: '', redirectTo: 'products', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
