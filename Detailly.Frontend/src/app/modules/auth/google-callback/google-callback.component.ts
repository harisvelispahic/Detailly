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
    const fragment = this.route.snapshot.fragment ?? '';
    const success = this.googleOAuth.handleCallback(fragment);

    if (!success) {
      this.router.navigate(['/auth/login']);
      return;
    }

    const defaultRoute = this.currentUser.getDefaultRoute();
    this.router.navigate([defaultRoute]);
  }
}
