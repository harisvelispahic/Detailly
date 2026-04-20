import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { AuthFacadeService } from './auth-facade.service';

@Injectable({ providedIn: 'root' })
export class GoogleOAuthService {
  private readonly authFacade = inject(AuthFacadeService);
  private readonly router = inject(Router);

  initiateLogin(): void {
    const returnUrl = `${window.location.origin}/auth/google-callback`;
    window.location.href = `${environment.apiUrl}/auth/external/google?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  handleCallback(fragment: string): boolean {
    const params = new URLSearchParams(fragment);

    const accessToken = params.get('accessToken');
    const refreshToken = params.get('refreshToken');

    if (!accessToken || !refreshToken) {
      return false;
    }

    this.authFacade.storeExternalLoginTokens(accessToken, refreshToken);
    window.history.replaceState(null, '', window.location.pathname);

    return true;
  }
}
