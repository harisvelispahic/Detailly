import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { myAuthData, myAuthGuard } from './core/guards/my-auth-guard';
import { oauthSetupGuard } from './core/guards/oauth-setup-guard';

const routes: Routes = [
  {
    path: 'admin',
    canActivate: [myAuthGuard, oauthSetupGuard],
    data: myAuthData({ requireAuth: true, requireAdmin: true }),
    loadChildren: () => import('./modules/admin/admin-module').then((m) => m.AdminModule),
  },
  {
    path: 'auth',
    // No oauthSetupGuard here — users must be able to reach /auth/setup and /auth/logout
    loadChildren: () => import('./modules/auth/auth-module').then((m) => m.AuthModule),
  },
  {
    path: 'staff',
    canActivate: [myAuthGuard, oauthSetupGuard],
    data: myAuthData({ requireAuth: true, requireStaff: true }),
    loadChildren: () => import('./modules/staff/staff-module').then((m) => m.StaffModule),
  },
  {
    path: 'client',
    canActivate: [myAuthGuard, oauthSetupGuard],
    data: myAuthData({ requireAuth: true }), // any authenticated user
    loadChildren: () => import('./modules/client/client-module').then((m) => m.ClientModule),
  },
  {
    path: '',
    canActivate: [oauthSetupGuard],
    loadChildren: () => import('./modules/public/public-module').then((m) => m.PublicModule),
  },
  // fallback 404
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'top' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
