import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { UsersApiService } from '../../../api-services/users/users-api.service';

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password');
  const confirm = control.get('confirmPassword');
  if (!password || !confirm) return null;
  return password.value !== confirm.value ? { passwordMismatch: true } : null;
}

function passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
  const value: string = control.value ?? '';
  if (!value) return null;
  if (!/[A-Z]/.test(value) || !/[a-z]/.test(value) || !/[0-9]/.test(value) || !/[^A-Za-z0-9]/.test(value)) {
    return { passwordStrength: true };
  }
  return null;
}

export interface PhoneCountry {
  name: string;
  iso2: string;
  dialCode: string;
}

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent extends BaseComponent implements OnInit {
  private fb = inject(FormBuilder);
  private usersApi = inject(UsersApiService);
  private router = inject(Router);

  hidePassword = true;
  hideConfirmPassword = true;
  registrationSuccess = false;

  // Sorted ascending by dialCode (numerically)
  phoneCountries: PhoneCountry[] = [
    { name: 'United States',          iso2: 'us', dialCode: '1'   },
    { name: 'Canada',                 iso2: 'ca', dialCode: '1'   },
    { name: 'Russia',                 iso2: 'ru', dialCode: '7'   },
    { name: 'South Africa',           iso2: 'za', dialCode: '27'  },
    { name: 'Greece',                 iso2: 'gr', dialCode: '30'  },
    { name: 'Netherlands',            iso2: 'nl', dialCode: '31'  },
    { name: 'Belgium',                iso2: 'be', dialCode: '32'  },
    { name: 'France',                 iso2: 'fr', dialCode: '33'  },
    { name: 'Spain',                  iso2: 'es', dialCode: '34'  },
    { name: 'Hungary',                iso2: 'hu', dialCode: '36'  },
    { name: 'Italy',                  iso2: 'it', dialCode: '39'  },
    { name: 'Romania',                iso2: 'ro', dialCode: '40'  },
    { name: 'Switzerland',            iso2: 'ch', dialCode: '41'  },
    { name: 'Austria',                iso2: 'at', dialCode: '43'  },
    { name: 'United Kingdom',         iso2: 'gb', dialCode: '44'  },
    { name: 'Denmark',                iso2: 'dk', dialCode: '45'  },
    { name: 'Sweden',                 iso2: 'se', dialCode: '46'  },
    { name: 'Norway',                 iso2: 'no', dialCode: '47'  },
    { name: 'Poland',                 iso2: 'pl', dialCode: '48'  },
    { name: 'Germany',                iso2: 'de', dialCode: '49'  },
    { name: 'Mexico',                 iso2: 'mx', dialCode: '52'  },
    { name: 'Brazil',                 iso2: 'br', dialCode: '55'  },
    { name: 'Australia',              iso2: 'au', dialCode: '61'  },
    { name: 'Japan',                  iso2: 'jp', dialCode: '81'  },
    { name: 'China',                  iso2: 'cn', dialCode: '86'  },
    { name: 'Turkey',                 iso2: 'tr', dialCode: '90'  },
    { name: 'India',                  iso2: 'in', dialCode: '91'  },
    { name: 'Albania',                iso2: 'al', dialCode: '355' },
    { name: 'Bulgaria',               iso2: 'bg', dialCode: '359' },
    { name: 'Ukraine',                iso2: 'ua', dialCode: '380' },
    { name: 'Serbia',                 iso2: 'rs', dialCode: '381' },
    { name: 'Montenegro',             iso2: 'me', dialCode: '382' },
    { name: 'Kosovo',                 iso2: 'xk', dialCode: '383' },
    { name: 'Croatia',                iso2: 'hr', dialCode: '385' },
    { name: 'Slovenia',               iso2: 'si', dialCode: '386' },
    { name: 'Bosnia & Herzegovina',   iso2: 'ba', dialCode: '387' },
    { name: 'North Macedonia',        iso2: 'mk', dialCode: '389' },
    { name: 'Czech Republic',         iso2: 'cz', dialCode: '420' },
    { name: 'Saudi Arabia',           iso2: 'sa', dialCode: '966' },
    { name: 'UAE',                    iso2: 'ae', dialCode: '971' },
  ];

  selectedCountry: PhoneCountry = this.phoneCountries.find(c => c.dialCode === '387')!;
  filteredCountries: PhoneCountry[] = [...this.phoneCountries];

  // Standalone control for the country autocomplete (not part of the main reactive form)
  countrySearchCtrl = new FormControl<PhoneCountry | string>(this.selectedCountry);

  // Phone digits (no spaces, no dial code)
  phoneDigits = '';
  phoneDisplay = '';

  form = this.fb.group(
    {
      firstName:       ['', [Validators.required, Validators.maxLength(100)]],
      lastName:        ['', [Validators.required, Validators.maxLength(100)]],
      username:        ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email:           ['', [Validators.required, Validators.email]],
      password:        ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
      confirmPassword: ['', [Validators.required]],
      isFleet:         [false],
      companyName:     [''],
    },
    { validators: passwordMatchValidator },
  );

  get isFleetChecked(): boolean {
    return !!this.form.get('isFleet')?.value;
  }

  get passwordMismatch(): boolean {
    const ctrl = this.form.get('confirmPassword');
    return this.form.hasError('passwordMismatch') && !!(ctrl?.dirty || ctrl?.touched);
  }

  // Used by mat-autocomplete [displayWith]
  displayCountry = (value: PhoneCountry | string | null): string => {
    if (!value) return '';
    if (typeof value === 'string') return value;
    return `+${value.dialCode}`;
  };

  ngOnInit(): void {
    this.form.get('isFleet')?.valueChanges.subscribe((fleet) => {
      const companyName = this.form.get('companyName');
      if (fleet) {
        companyName?.setValidators([Validators.required, Validators.maxLength(200)]);
      } else {
        companyName?.clearValidators();
        companyName?.setValue('');
      }
      companyName?.updateValueAndValidity();
    });

    this.countrySearchCtrl.valueChanges.subscribe((value) => {
      if (typeof value === 'string') {
        const search = value.replace(/^\+/, '').trim();
        this.filteredCountries = search
          ? this.phoneCountries.filter((c) => c.dialCode.startsWith(search))
          : [...this.phoneCountries];
      } else {
        // Object selected — show all
        this.filteredCountries = [...this.phoneCountries];
      }
    });
  }

  onCountryFocus(): void {
    // Clear so user can type a fresh dial code without having to delete first
    this.countrySearchCtrl.setValue('', { emitEvent: true });
  }

  onCountryBlur(): void {
    // Restore display to selected country if user didn't pick one
    setTimeout(() => {
      if (typeof this.countrySearchCtrl.value !== 'object') {
        this.countrySearchCtrl.setValue(this.selectedCountry, { emitEvent: false });
      }
    }, 250);
  }

  onCountrySelected(event: MatAutocompleteSelectedEvent): void {
    this.selectedCountry = event.option.value as PhoneCountry;
  }

  onPhoneInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const digits = input.value.replace(/\D/g, '').slice(0, 15);
    this.phoneDigits = digits;
    this.phoneDisplay = this.formatPhoneDigits(digits);
    input.value = this.phoneDisplay;
  }

  private formatPhoneDigits(digits: string): string {
    if (digits.length <= 2) return digits;

    const g1   = digits.slice(0, 2);   // always 2 digits
    const g2   = digits.slice(2, 5);   // up to 3 digits
    const rest = digits.slice(5);      // remaining

    const parts: string[] = [g1];
    if (g2) parts.push(g2);

    if (rest) {
      if (rest.length <= 3) {
        parts.push(rest);              // ≤3 remaining → keep as one group
      } else {
        for (let i = 0; i < rest.length; i += 2) {
          parts.push(rest.slice(i, i + 2));  // 4+ remaining → pairs
        }
      }
    }

    return parts.join(' ');
  }

  get cleanPhoneValue(): string | null {
    if (!this.phoneDigits) return null;
    return `+${this.selectedCountry.dialCode}${this.phoneDigits}`;
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) return;
    this.startLoading();

    const v = this.form.value;

    this.usersApi
      .create({
        firstName:   v.firstName!.trim(),
        lastName:    v.lastName!.trim(),
        username:    v.username!.trim(),
        email:       v.email!.trim().toLowerCase(),
        password:    v.password!,
        phone:       this.cleanPhoneValue,
        isFleet:     v.isFleet ?? false,
        companyName: v.companyName?.trim() || null,
      })
      .subscribe({
        next: () => {
          this.stopLoading();
          this.registrationSuccess = true;
        },
        error: (err) => {
          const message =
            err?.error?.message ??
            err?.error?.detail ??
            err?.error?.title ??
            'Registration failed. Please try again.';
          this.stopLoading(message);
        },
      });
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
