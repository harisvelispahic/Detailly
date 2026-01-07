import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckoutComponent } from './checkout/checkout.component';

const routes: Routes = [
  {
    path: 'checkout/:bookingId',
    component: CheckoutComponent,
    data: { requireAuth: true }, // optional — only if you want it protected
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClientRoutingModule {}
