import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WalletTopUpComponent } from './wallet-topup/wallet-topup.component';
import { myAuthGuard } from '../../core/guards/my-auth-guard';
import { OrderPaymentComponent } from './orders/order-payment/order-payment.component';
import { MyOrdersComponent } from './orders/my-orders/my-orders.component';
import { OrderDetailsComponent } from './orders/order-details/order-details.component';
import { MyBookingsPageComponent } from './bookings/my-bookings-page/my-bookings-page.component';
import { BookingDetailsPageComponent } from './bookings/booking-details-page/booking-details-page.component';
import { BookingPayCardPageComponent } from './bookings/booking-pay-card-page/booking-pay-card-page.component';

const routes: Routes = [
  // PAYMENTS
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

  // ORDERS
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

  // BOOKINGS
  {
    path: 'bookings',
    component: MyBookingsPageComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
  },
  {
    path: 'bookings/:id',
    component: BookingDetailsPageComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
  },
  {
    path: 'bookings/pay/:bookingId',
    component: BookingPayCardPageComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClientRoutingModule {}
