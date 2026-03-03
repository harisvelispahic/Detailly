// src/app/core/guards/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, Router } from '@angular/router';
import { CurrentUserService } from '../services/auth/current-user.service';

export const myAuthGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const currentUser = inject(CurrentUserService);
  const router = inject(Router);

  const data = route.data['auth'] as MyAuthRouteData | undefined;

  const requireAuth = data?.requireAuth === true;
  const requireAdmin = data?.requireAdmin === true;
  const requireManager = data?.requireManager === true;
  const requireEmployee = data?.requireEmployee === true;
  const requireFleet = data?.requireFleet === true;
  const requireStandard = data?.requireStandard === true;

  const isAuth = currentUser.isAuthenticated();

  // 1) ako ruta traži auth, a user nije logiran → login
  if (requireAuth && !isAuth) {
    router.navigate(['/auth/login']);
    return false;
  }

  // Ako ne traži auth → pusti (javne rute)
  if (!requireAuth) {
    return true;
  }

  // 2) role check – admin > manager > employee
  const user = currentUser.snapshot;
  if (!user) {
    router.navigate(['/auth/login']);
    return false;
  }

  if (requireAdmin && !user.isAdmin) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  if (requireManager && !user.isManager) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  if (requireEmployee && !user.isEmployee) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  if (requireFleet && !user.isFleet) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  if (requireStandard && !user.isStandard) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  return true;
};

export interface MyAuthRouteData {
  requireAuth?: boolean;
  requireAdmin?: boolean;
  requireManager?: boolean;
  requireEmployee?: boolean;
  requireFleet?: boolean;
  requireStandard?: boolean;
}

export function myAuthData(data: MyAuthRouteData): { auth: MyAuthRouteData } {
  return { auth: data };
}
