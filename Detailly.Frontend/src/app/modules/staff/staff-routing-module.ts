import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';

const routes: Routes = [{ path: '', component: StaffHomeComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StaffRoutingModule {}
