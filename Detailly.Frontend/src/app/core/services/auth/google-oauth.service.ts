import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { AuthFacadeService } from './auth-facade.service';

export interface GoogleCallbackResult {
  success: boolean;
  isSetupRequired: boolean;
}

@Injectable({ providedIn: 'root' })
export class GoogleOAuthService {
  private readonly authFacade = inject(AuthFacadeService);
  private readonly router = inject(Router);

  initiateLogin(): void {
    const returnUrl = `${window.location.origin}/auth/google-callback`;
    window.location.href = `${environment.apiUrl}/auth/external/google?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  handleCallback(fragment: string): GoogleCallbackResult {
    const params = new URLSearchParams(fragment);

    const accessToken = params.get('accessToken');
    const refreshToken = params.get('refreshToken');

    if (!accessToken || !refreshToken) {
      return { success: false, isSetupRequired: false };
    }

    this.authFacade.storeExternalLoginTokens(accessToken, refreshToken);
    window.history.replaceState(null, '', window.location.pathname);

    const isSetupRequired = params.get('isSetupRequired') === 'true';
    return { success: true, isSetupRequired };
  }
}
