import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StaffLayoutComponent } from './staff-layout/staff-layout.component';
import { StaffHomeComponent } from './pages/staff-home/staff-home.component';

const routes: Routes = [
  {
    path: '',
    component: StaffLayoutComponent,
    children: [
      { path: '', component: StaffHomeComponent },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StaffRoutingModule {}
