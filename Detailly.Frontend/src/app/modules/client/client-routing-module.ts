import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckoutComponent } from './checkout/checkout.component';
import { WalletTopUpComponent } from './wallet-topup/wallet-topup.component';
import { myAuthGuard } from '../../core/guards/my-auth-guard';

const routes: Routes = [
  {
    path: 'checkout/:bookingId',
    component: CheckoutComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true }, // optional — only if you want it protected
  },
  {
    path: 'wallet/top-up',
    component: WalletTopUpComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClientRoutingModule {}
