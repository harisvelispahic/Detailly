import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ClientRoutingModule } from './client-routing-module';
import { SharedModule } from '../shared/shared-module';
import { CheckoutComponent } from './checkout/checkout.component';
import { WalletTopUpComponent } from './wallet-topup/wallet-topup.component';
import { OrderPaymentComponent } from './order-payment/order-payment.component';
import { MyOrdersComponent } from './my-orders/my-orders.component';
import { OrderDetailsComponent } from './order-details/order-details.component';

@NgModule({
  declarations: [CheckoutComponent, WalletTopUpComponent, OrderPaymentComponent, MyOrdersComponent, OrderDetailsComponent],
  imports: [SharedModule, ClientRoutingModule, FormsModule],
  exports: [FormsModule],
})
export class ClientModule {}
