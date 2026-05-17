import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStorageService } from '../services/auth/auth-storage.service';
import { AuthFacadeService } from '../services/auth/auth-facade.service';

export const oauthSetupGuard: CanActivateFn = () => {
  const router = inject(Router);
  const auth = inject(AuthFacadeService);

  // Only block authenticated users who still need setup.
  if (
    auth.isAuthenticated() &&
    localStorage.getItem(AuthStorageService.SETUP_REQUIRED_KEY) === 'true'
  ) {
    router.navigate(['/auth/setup']);
    return false;
  }

  return true;
};
