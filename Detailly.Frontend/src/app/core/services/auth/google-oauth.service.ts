import { Injectable, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { AuthFacadeService } from './auth-facade.service';
import { AuthStorageService } from './auth-storage.service';

export interface GoogleCallbackResult {
  success: boolean;
  isSetupRequired: boolean;
  requiresLinking: boolean;
  pendingLinkToken: string | null;
}

export const PENDING_LINK_TOKEN_KEY = 'pending_link_token';

@Injectable({ providedIn: 'root' })
export class GoogleOAuthService {
  private readonly authFacade = inject(AuthFacadeService);

  initiateLogin(): void {
    const returnUrl = `${window.location.origin}/auth/google-callback`;
    window.location.href = `${environment.apiUrl}/auth/external/google?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  handleCallback(fragment: string): GoogleCallbackResult {
    const params = new URLSearchParams(fragment);

    if (params.get('requiresLinking') === 'true') {
      const pendingLinkToken = params.get('pendingLinkToken');
      window.history.replaceState(null, '', window.location.pathname);

      if (!pendingLinkToken) {
        return { success: false, isSetupRequired: false, requiresLinking: false, pendingLinkToken: null };
      }

      sessionStorage.setItem(PENDING_LINK_TOKEN_KEY, pendingLinkToken);
      return { success: true, isSetupRequired: false, requiresLinking: true, pendingLinkToken };
    }

    const accessToken = params.get('accessToken');
    const refreshToken = params.get('refreshToken');

    if (!accessToken || !refreshToken) {
      return { success: false, isSetupRequired: false, requiresLinking: false, pendingLinkToken: null };
    }

    this.authFacade.storeExternalLoginTokens(accessToken, refreshToken);
    window.history.replaceState(null, '', window.location.pathname);

    const isSetupRequired = params.get('isSetupRequired') === 'true';

    if (isSetupRequired) {
      localStorage.setItem(AuthStorageService.SETUP_REQUIRED_KEY, 'true');
    }

    return { success: true, isSetupRequired, requiresLinking: false, pendingLinkToken: null };
  }
}
