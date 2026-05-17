import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, tap, catchError, map } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import {
  LoginCommand,
  LoginCommandDto,
  LogoutCommand,
  RefreshTokenCommand,
  RefreshTokenCommandDto,
} from '../../../api-services/auth/auth-api.model';

import { AuthStorageService } from './auth-storage.service';
import { CurrentUserDto } from './current-user.dto';
import { JwtPayloadDto } from './jwt-payload.dto';
import { REFRESH_TOKEN_EXPIRY_DAYS } from './auth.constants';

import * as Sentry from '@sentry/angular';

/**
 * Main auth service (facade).
 * - talks to AuthApiService (HTTP)
 * - talks to AuthStorageService (localStorage)
 * - decodes JWT and holds CurrentUser as a signal
 *
 * Used in:
 * - interceptor (getAccessToken, refresh)
 * - guards (isAuthenticated, isAdmin)
 * - components (login, logout, navbar)
 */
@Injectable({ providedIn: 'root' })
export class AuthFacadeService {
  private api = inject(AuthApiService);
  private storage = inject(AuthStorageService);
  private router = inject(Router);

  // === REACTIVE STATE: current user ===

  private _currentUser = signal<CurrentUserDto | null>(null);

  /** Read-only signal for the UI – consumed as auth.currentUser() */
  currentUser = this._currentUser.asReadonly();

  /** Computed signals derived from the current user. */
  isAuthenticated = computed(() => !!this._currentUser());
  isAdmin = computed(() => this._currentUser()?.isAdmin ?? false);
  isManager = computed(() => this._currentUser()?.isManager ?? false);
  isEmployee = computed(() => this._currentUser()?.isEmployee ?? false);
  isFleet = computed(() => this._currentUser()?.isFleet ?? false);
  isStandard = computed(() => this._currentUser()?.isStandard ?? false);

  constructor() {
    // attempt to initialize from existing access token
    this.initializeFromToken();
  }

  // =========================================================
  // PUBLIC API
  // =========================================================

  /**
   * Logs in a user (email + password).
   * Saves tokens to storage, decodes JWT and populates current user state.
   */
  login(payload: LoginCommand): Observable<void> {
    return this.api.login(payload).pipe(
      tap((response: LoginCommandDto) => {
        this.storage.saveLogin(response); // access + refresh + expiries
        this.decodeAndSetUser(response.accessToken); // populate _currentUser
      }),
      map(() => void 0),
    );
  }

  /**
   * Logs out the user:
   * - clears state and tokens locally
   * - attempts to invalidate refresh token on the server (ignores errors)
   */
  logout(): Observable<void> {
    const refreshToken = this.storage.getRefreshToken();

    // 1) clear locally (optimistic logout)
    this.clearUserState();

    // 2) no refresh token → skip API call
    if (!refreshToken) {
      return of(void 0);
    }

    const payload: LogoutCommand = { refreshToken };

    // 3) attempt server-side logout, ignore errors
    return this.api.logout(payload).pipe(catchError(() => of(void 0)));
  }

  /**
   * Refreshes the access token using the refresh token.
   * Called by the interceptor on 401 responses.
   */
  refresh(payload: RefreshTokenCommand): Observable<RefreshTokenCommandDto> {
    return this.api.refresh(payload).pipe(
      tap((response: RefreshTokenCommandDto) => {
        this.storage.saveRefresh(response); // save new tokens
        this.decodeAndSetUser(response.accessToken); // update current user
      }),
    );
  }

  /**
   * Stores tokens received from the Google OAuth redirect fragment.
   */
  storeExternalLoginTokens(accessToken: string, refreshToken: string): void {
    const payload = jwtDecode<{ exp: number }>(accessToken);
    const accessExpiry = new Date(payload.exp * 1000).toISOString();
    const refreshExpiry = new Date(
      Date.now() + REFRESH_TOKEN_EXPIRY_DAYS * 24 * 60 * 60 * 1000,
    ).toISOString();

    this.storage.saveLogin({
      accessToken,
      refreshToken,
      accessTokenExpiresAtUtc: accessExpiry,
      refreshTokenExpiresAtUtc: refreshExpiry,
    });

    this.decodeAndSetUser(accessToken);
  }

  /**
   * Utility for guards/interceptors – clears auth state and redirects to /login.
   */
  redirectToLogin(): void {
    this.clearUserState();
    this.router.navigate(['/auth/login']);
  }

  // =========================================================
  // GETTERS FOR INTERCEPTOR
  // =========================================================

  /**
   * Access token for the Authorization header.
   */
  getAccessToken(): string | null {
    return this.storage.getAccessToken();
  }

  /**
   * Refresh token for the refresh call.
   */
  getRefreshToken(): string | null {
    return this.storage.getRefreshToken();
  }

  // =========================================================
  // PRIVATE HELPERS
  // =========================================================

  /**
   * On application startup (constructor) – attempt to restore state from the existing token.
   */
  private initializeFromToken(): void {
    const token = this.storage.getAccessToken();
    if (token) {
      this.decodeAndSetUser(token);
    }
  }

  /**
   * Decodes a JWT and sets the current user state.
   */
  private decodeAndSetUser(token: string): void {
    try {
      const payload = jwtDecode<JwtPayloadDto>(token);

      const idStr =
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
        payload.sub ??
        null;

      const email =
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? null;

      const isAdmin = String(payload.is_admin).toLowerCase() === 'true';
      const isManager = String(payload.is_manager).toLowerCase() === 'true';
      const isEmployee = String(payload.is_employee).toLowerCase() === 'true';
      const isFleet = String(payload.is_fleet).toLowerCase() === 'true';

      const user: CurrentUserDto = {
        userId: Number(idStr),
        email: email ?? '',

        isAdmin,
        isManager,
        isEmployee,
        isFleet,
        isStandard: !isFleet,
        isStaff: isAdmin || isManager || isEmployee,
        isAdminOrManager: isAdmin || isManager,

        tokenVersion: Number(payload.ver ?? 0),
      };

      // optional: if token is missing id, treat as invalid
      if (!user.userId || Number.isNaN(user.userId)) {
        this._currentUser.set(null);
        return;
      }

      this._currentUser.set(user);

      Sentry.setUser({
        id: String(user.userId),
        email: user.email || undefined,
      });

      Sentry.setTag('is_admin', String(user.isAdmin));
      Sentry.setTag('is_manager', String(user.isManager));
      Sentry.setTag('is_employee', String(user.isEmployee));
      Sentry.setTag('is_fleet', String(user.isFleet));
      Sentry.setTag('area', 'frontend');
    } catch (error) {
      this._currentUser.set(null);
    }
  }

  /**
   * Clears user state and all tokens from storage.
   */
  private clearUserState(): void {
    this._currentUser.set(null);
    this.storage.clear();
    Sentry.setUser(null);
  }
}
