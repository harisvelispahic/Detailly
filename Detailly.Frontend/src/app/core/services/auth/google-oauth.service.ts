import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
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
  private readonly authApi = inject(AuthApiService);

  initiateLogin(): void {
    const returnUrl = `${window.location.origin}/auth/google-callback`;
    window.location.href = `${environment.apiUrl}/auth/external/google?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  handleCallback(code: string): Observable<GoogleCallbackResult> {
    return this.authApi.exchangeOAuthCode({ code }).pipe(
      map(result => {
        if (result.requiresLinking) {
          if (!result.pendingLinkToken) {
            return { success: false, isSetupRequired: false, requiresLinking: false, pendingLinkToken: null };
          }
          sessionStorage.setItem(PENDING_LINK_TOKEN_KEY, result.pendingLinkToken);
          return { success: true, isSetupRequired: false, requiresLinking: true, pendingLinkToken: result.pendingLinkToken };
        }

        if (!result.accessToken || !result.refreshToken) {
          return { success: false, isSetupRequired: false, requiresLinking: false, pendingLinkToken: null };
        }

        this.authFacade.storeExternalLoginTokens(result.accessToken, result.refreshToken);

        if (result.isSetupRequired) {
          localStorage.setItem(AuthStorageService.SETUP_REQUIRED_KEY, 'true');
        }

        return { success: true, isSetupRequired: result.isSetupRequired, requiresLinking: false, pendingLinkToken: null };
      })
    );
  }
}
