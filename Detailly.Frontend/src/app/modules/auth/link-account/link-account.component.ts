import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { PENDING_LINK_TOKEN_KEY } from '../../../core/services/auth/google-oauth.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-link-account',
  standalone: false,
  templateUrl: './link-account.component.html',
  styleUrl: './link-account.component.scss',
})
export class LinkAccountComponent extends BaseComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly authApi = inject(AuthApiService);
  private readonly authFacade = inject(AuthFacadeService);
  private readonly toaster = inject(ToasterService);

  hidePassword = true;

  form = this.fb.group({
    password: ['', [Validators.required]],
  });

  private pendingLinkToken: string | null = null;

  ngOnInit(): void {
    this.pendingLinkToken = sessionStorage.getItem(PENDING_LINK_TOKEN_KEY);
    if (!this.pendingLinkToken) {
      this.router.navigate(['/auth/login']);
    }
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading || !this.pendingLinkToken) return;
    this.startLoading();

    this.authApi.linkExternalAccount({
      pendingLinkToken: this.pendingLinkToken,
      password: this.form.value.password!,
    }).subscribe({
      next: (response) => {
        sessionStorage.removeItem(PENDING_LINK_TOKEN_KEY);
        this.authFacade.storeExternalLoginTokens(response.accessToken, response.refreshToken);
        this.stopLoading();
        this.router.navigate(['/']);
      },
      error: () => {
        this.toaster.error('Wrong password. Account linking was unsuccessful.');
        this.stopLoading();
      },
    });
  }

  cancel(): void {
    sessionStorage.removeItem(PENDING_LINK_TOKEN_KEY);
    this.router.navigate(['/auth/login']);
  }
}
