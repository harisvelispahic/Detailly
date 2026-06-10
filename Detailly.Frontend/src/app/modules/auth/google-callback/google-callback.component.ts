import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GoogleOAuthService } from '../../../core/services/auth/google-oauth.service';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';

@Component({
  selector: 'app-google-callback',
  standalone: false,
  template: `<p style="color: white; padding: 2rem;">Logging you in...</p>`,
})
export class GoogleCallbackComponent implements OnInit {
  private readonly googleOAuth = inject(GoogleOAuthService);
  private readonly currentUser = inject(CurrentUserService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  ngOnInit(): void {
    const code = this.route.snapshot.queryParamMap.get('code');

    if (!code) {
      this.router.navigate(['/auth/login']);
      return;
    }

    this.googleOAuth.handleCallback(code).subscribe({
      next: result => {
        if (!result.success) {
          this.router.navigate(['/auth/login']);
          return;
        }

        if (result.requiresLinking) {
          this.router.navigate(['/auth/link-account']);
          return;
        }

        if (result.isSetupRequired) {
          this.router.navigate(['/auth/setup']);
          return;
        }

        const defaultRoute = this.currentUser.getDefaultRoute();
        this.router.navigate([defaultRoute]);
      },
      error: () => {
        this.router.navigate(['/auth/login']);
      },
    });
  }
}
