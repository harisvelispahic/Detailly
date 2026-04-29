import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DashboardLayoutComponent } from '../shared/components/dashboard-layout/dashboard-layout.component';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';
import { ShiftsComponent } from './pages/shifts/shifts.component';
import { MyShiftsComponent } from './pages/my-shifts/my-shifts.component';
import { StaffMembersComponent } from './pages/staff-members/staff-members.component';
import { LocationsComponent } from './pages/locations/locations.component';
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
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StaffRoutingModule {}
