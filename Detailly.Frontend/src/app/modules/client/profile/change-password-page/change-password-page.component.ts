import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';

function passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
  const value: string = control.value ?? '';
  if (!value) return null;
  const errors: ValidationErrors = {};
  if (!/[A-Z]/.test(value)) errors['passwordNoUppercase'] = true;
  if (!/[a-z]/.test(value)) errors['passwordNoLowercase'] = true;
  if (!/[0-9]/.test(value)) errors['passwordNoDigit'] = true;
  if (!/[^A-Za-z0-9]/.test(value)) errors['passwordNoSpecial'] = true;
  return Object.keys(errors).length ? errors : null;
}

function confirmPasswordValidator(control: AbstractControl): ValidationErrors | null {
  const parent = control.parent as FormGroup;
  if (!parent) return null;
  return control.value === parent.get('newPassword')?.value ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-change-password-page',
  standalone: false,
  templateUrl: './change-password-page.component.html',
  styleUrl: './change-password-page.component.scss',
})
export class ChangePasswordPageComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isLoadingUser = true;
  isOAuth = false;
  error?: string;

  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthFacadeService,
    private users: UsersApiService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    const userId = this.auth.currentUser()?.userId;
    if (!userId) {
      this.router.navigate(['/client/profile']);
      return;
    }

    this.users.getById(userId).subscribe({
      next: (user) => {
        this.isOAuth = user.isOAuthUser;
        this.isLoadingUser = false;
      },
      error: () => {
        this.isLoadingUser = false;
      },
    });

    this.form = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
      confirmPassword: ['', [Validators.required, confirmPasswordValidator]],
    });

    this.form.get('newPassword')?.valueChanges.subscribe(() => {
      this.form.get('confirmPassword')?.updateValueAndValidity();
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.error = undefined;
    this.isLoading = true;

    this.users.changePassword({
      currentPassword: this.form.value.currentPassword,
      newPassword: this.form.value.newPassword,
    }).subscribe({
      next: () => {
        this.auth.logout().subscribe(() => {
          this.router.navigate(['/auth/login'], {
            queryParams: { message: 'password-changed' },
          });
        });
      },
      error: (err) => {
        this.error = err?.error?.message ?? 'Failed to change password. Please try again.';
        this.isLoading = false;
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/client/profile']);
  }
}
