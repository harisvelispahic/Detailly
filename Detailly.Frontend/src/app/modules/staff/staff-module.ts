import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StaffRoutingModule } from './staff-routing-module';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';

@NgModule({
  declarations: [StaffHomeComponent],
  imports: [CommonModule, StaffRoutingModule],
})
export class StaffModule {}
