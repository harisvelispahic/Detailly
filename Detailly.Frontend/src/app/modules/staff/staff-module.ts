import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared-module';
import { StaffRoutingModule } from './staff-routing-module';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';
import { ShiftsComponent } from './pages/shifts/shifts.component';
import { ShiftUpsertDialogComponent } from './pages/shifts/shift-upsert-dialog/shift-upsert-dialog.component';
import { MyShiftsComponent } from './pages/my-shifts/my-shifts.component';
import { StaffMembersComponent } from './pages/staff-members/staff-members.component';
import { StaffMemberUpsertDialogComponent } from './pages/staff-members/staff-member-upsert-dialog/staff-member-upsert-dialog.component';
import { LocationsComponent } from './pages/locations/locations.component';
import { LocationUpsertDialogComponent } from './pages/locations/location-upsert-dialog/location-upsert-dialog.component';
import { LocationDetailDialogComponent } from './pages/locations/location-detail-dialog/location-detail-dialog.component';
import { ServicePackagesComponent } from './pages/service-packages/service-packages.component';
import { ServicePackageUpsertDialogComponent } from './pages/service-packages/service-package-upsert-dialog/service-package-upsert-dialog.component';
import { ServicePackageItemsPageComponent } from './pages/service-packages/service-package-items-page/service-package-items-page.component';
import { ServicePackageItemUpsertDialogComponent } from './pages/service-packages/service-package-items-page/service-package-item-upsert-dialog/service-package-item-upsert-dialog.component';
import { VehicleCategoriesComponent } from './pages/vehicle-categories/vehicle-categories.component';
import { VehicleCategoryUpsertDialogComponent } from './pages/vehicle-categories/vehicle-category-upsert-dialog/vehicle-category-upsert-dialog.component';

@NgModule({
  declarations: [
    StaffHomeComponent,
    ShiftsComponent,
    ShiftUpsertDialogComponent,
    MyShiftsComponent,
    StaffMembersComponent,
    StaffMemberUpsertDialogComponent,
    LocationsComponent,
    LocationUpsertDialogComponent,
    LocationDetailDialogComponent,
    ServicePackagesComponent,
    ServicePackageUpsertDialogComponent,
    ServicePackageItemsPageComponent,
    ServicePackageItemUpsertDialogComponent,
    VehicleCategoriesComponent,
    VehicleCategoryUpsertDialogComponent,
  ],
  imports: [SharedModule, StaffRoutingModule],
})
export class StaffModule {}
