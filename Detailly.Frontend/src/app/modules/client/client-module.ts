import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ClientRoutingModule } from './client-routing-module';
import { SharedModule } from '../shared/shared-module';
import { CheckoutComponent } from './checkout/checkout.component';
import { WalletTopUpComponent } from './wallet-topup/wallet-topup.component';

@NgModule({
  declarations: [CheckoutComponent, WalletTopUpComponent],
  imports: [SharedModule, ClientRoutingModule, FormsModule],
  exports: [FormsModule],
})
export class ClientModule {}
