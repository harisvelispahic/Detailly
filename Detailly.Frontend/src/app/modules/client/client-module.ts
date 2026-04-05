import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ClientRoutingModule } from './client-routing-module';
import { SharedModule } from '../shared/shared-module';
import { WalletTopUpComponent } from './wallet-topup/wallet-topup.component';
import { OrderPaymentComponent } from './orders/order-payment/order-payment.component';
import { MyOrdersComponent } from './orders/my-orders/my-orders.component';
import { OrderDetailsComponent } from './orders/order-details/order-details.component';
import { MyBookingsPageComponent } from './bookings/my-bookings-page/my-bookings-page.component';
import { BookingDetailsPageComponent } from './bookings/booking-details-page/booking-details-page.component';
import { BookingPayCardPageComponent } from './bookings/booking-pay-card-page/booking-pay-card-page.component';
import { ClientLayoutComponent } from './client-layout/client-layout.component';
import { ProfilePageComponent } from './profile/profile-page/profile-page.component';

@NgModule({
  declarations: [
    ClientLayoutComponent,
    WalletTopUpComponent,
    OrderPaymentComponent,
    MyOrdersComponent,
    OrderDetailsComponent,
    MyBookingsPageComponent,
    BookingDetailsPageComponent,
    BookingPayCardPageComponent,
    ProfilePageComponent,
  ],
  imports: [SharedModule, ClientRoutingModule, FormsModule],
  exports: [FormsModule],
})
export class ClientModule {}
