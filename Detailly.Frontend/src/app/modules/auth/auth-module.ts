import {NgModule} from '@angular/core';

import {AuthRoutingModule} from './auth-routing-module';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {LoginComponent} from './login/login.component';
import {RegisterComponent} from './register/register.component';
import {ForgotPasswordComponent} from './forgot-password/forgot-password.component';
import {LogoutComponent} from './logout/logout.component';
import {GoogleCallbackComponent} from './google-callback/google-callback.component';
import {OAuthSetupComponent} from './oauth-setup/oauth-setup.component';
import {LinkAccountComponent} from './link-account/link-account.component';
import {SharedModule} from '../shared/shared-module';


@NgModule({
  declarations: [
    AuthLayoutComponent,
    LoginComponent,
    RegisterComponent,
    ForgotPasswordComponent,
    LogoutComponent,
    GoogleCallbackComponent,
    OAuthSetupComponent,
    LinkAccountComponent,
  ],
  imports: [
    AuthRoutingModule,
    SharedModule
  ]
})
export class AuthModule { }
