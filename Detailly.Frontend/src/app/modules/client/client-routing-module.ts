import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckoutComponent } from './checkout/checkout.component';
import { WalletTopUpComponent } from './wallet-topup/wallet-topup.component';
import { myAuthGuard } from '../../core/guards/my-auth-guard';
import { OrderPaymentComponent } from './orders/order-payment/order-payment.component';
import { MyOrdersComponent } from './orders/my-orders/my-orders.component';
import { OrderDetailsComponent } from './orders/order-details/order-details.component';

const routes: Routes = [
  {
    path: 'bookings/checkout/:bookingId',
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
  {
    path: 'orders/checkout/:orderId',
    component: OrderPaymentComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
  },
  {
    path: 'orders',
    component: MyOrdersComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
  },
  {
    path: 'orders/:id',
    component: OrderDetailsComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClientRoutingModule {}
