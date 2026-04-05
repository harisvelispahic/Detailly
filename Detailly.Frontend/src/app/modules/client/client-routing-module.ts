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
import { ClientLayoutComponent } from './client-layout/client-layout.component';
import { ProfilePageComponent } from './profile/profile-page/profile-page.component';

const routes: Routes = [
  {
    path: '',
    component: ClientLayoutComponent,
    children: [
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
      {
        path: 'profile',
        component: ProfilePageComponent,
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
      },
      {
        path: '',
        redirectTo: 'profile',
        pathMatch: 'full',
      },
    ],
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClientRoutingModule {}
