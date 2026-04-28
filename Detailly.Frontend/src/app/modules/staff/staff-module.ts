import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared-module';
import { StaffRoutingModule } from './staff-routing-module';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';
import { ShiftsComponent } from './pages/shifts/shifts.component';
import { ShiftUpsertDialogComponent } from './pages/shifts/shift-upsert-dialog/shift-upsert-dialog.component';
import { MyShiftsComponent } from './pages/my-shifts/my-shifts.component';

@NgModule({
  declarations: [
    StaffHomeComponent,
    ShiftsComponent,
    ShiftUpsertDialogComponent,
    MyShiftsComponent,
  ],
  imports: [SharedModule, StaffRoutingModule],
})
export class StaffModule {}
