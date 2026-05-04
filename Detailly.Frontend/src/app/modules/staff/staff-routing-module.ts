import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DashboardLayoutComponent } from '../shared/components/dashboard-layout/dashboard-layout.component';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';
import { ShiftsComponent } from './pages/shifts/shifts.component';
import { MyShiftsComponent } from './pages/my-shifts/my-shifts.component';
import { StaffMembersComponent } from './pages/staff-members/staff-members.component';
import { LocationsComponent } from './pages/locations/locations.component';
import { ServicePackagesComponent } from './pages/service-packages/service-packages.component';
import { ServicePackageItemsPageComponent } from './pages/service-packages/service-package-items-page/service-package-items-page.component';
import { VehicleCategoriesComponent } from './pages/vehicle-categories/vehicle-categories.component';
import { AssignBookingsComponent } from './pages/assign-bookings/assign-bookings.component';
import { MyAssignedBookingsComponent } from './pages/my-assigned-bookings/my-assigned-bookings.component';
import { myAuthData, myAuthGuard } from '../../core/guards/my-auth-guard';

const routes: Routes = [
  {
    path: '',
    component: DashboardLayoutComponent,
    children: [
      { path: '', component: StaffHomeComponent },
      {
        path: 'shifts',
        component: ShiftsComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireAdminOrManager: true }),
      },
      {
        path: 'my-shifts',
        component: MyShiftsComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireEmployee: true }),
      },
      {
        path: 'staff-members',
        component: StaffMembersComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireAdminOrManager: true }),
      },
      {
        path: 'locations',
        component: LocationsComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true }),
      },
      {
        path: 'service-packages',
        component: ServicePackagesComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireAdminOrManager: true }),
      },
      {
        path: 'service-packages/items',
        component: ServicePackageItemsPageComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireAdminOrManager: true }),
      },
      {
        path: 'vehicle-categories',
        component: VehicleCategoriesComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireAdminOrManager: true }),
      },
      {
        path: 'assign-bookings',
        component: AssignBookingsComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireAdminOrManager: true }),
      },
      {
        path: 'my-bookings',
        component: MyAssignedBookingsComponent,
        canActivate: [myAuthGuard],
        data: myAuthData({ requireAuth: true, requireEmployee: true }),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StaffRoutingModule {}
