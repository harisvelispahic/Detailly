import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared-module';
import { StaffRoutingModule } from './staff-routing-module';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';

@NgModule({
  declarations: [StaffHomeComponent],
  imports: [SharedModule, StaffRoutingModule],
})
export class StaffModule {}
