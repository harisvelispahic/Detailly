import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { GetUserByIdQueryDto } from '../../../../api-services/users/users-api.model';

export interface EditProfileDialogData {
  user: GetUserByIdQueryDto;
  isFleet: boolean;
}

export interface PhoneCountry {
  name: string;
  iso2: string;
  dialCode: string;
}

const PHONE_COUNTRIES: PhoneCountry[] = [
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

@Component({
  selector: 'app-edit-profile-dialog',
  standalone: false,
  templateUrl: './edit-profile-dialog.component.html',
  styleUrl: './edit-profile-dialog.component.scss',
})
export class EditProfileDialogComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  error?: string;

  phoneCountries: PhoneCountry[] = PHONE_COUNTRIES;
  selectedCountry: PhoneCountry = PHONE_COUNTRIES.find(c => c.dialCode === '387')!;
  filteredCountries: PhoneCountry[] = [...PHONE_COUNTRIES];
  countrySearchCtrl = new FormControl<PhoneCountry | string>(this.selectedCountry);
  phoneDigits = '';
  phoneDisplay = '';

  get isFleet(): boolean {
    return this.data.isFleet;
  }

  get isOAuth(): boolean {
    return this.data.user.isOAuthUser;
  }

  displayCountry = (value: PhoneCountry | string | null): string => {
    if (!value) return '';
    if (typeof value === 'string') return value;
    return `+${value.dialCode}`;
  };

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditProfileDialogComponent>,
    private users: UsersApiService,
    private router: Router,
    @Inject(MAT_DIALOG_DATA) public data: EditProfileDialogData,
  ) {}

  ngOnInit(): void {
    const u = this.data.user;
    this.form = this.fb.group({
      firstName: [u.firstName, [Validators.required, Validators.maxLength(100)]],
      lastName: [u.lastName, [Validators.required, Validators.maxLength(100)]],
      username: [u.username, [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: [{ value: u.email, disabled: u.isOAuthUser }, u.isOAuthUser ? [] : [Validators.required, Validators.email]],
      ...(this.data.isFleet ? { companyName: [u.companyName ?? ''] } : {}),
    });

    if (u.phone) {
      const stripped = u.phone.startsWith('+') ? u.phone.slice(1) : u.phone;
      const sorted = [...PHONE_COUNTRIES].sort((a, b) => b.dialCode.length - a.dialCode.length);
      const match = sorted.find(c => stripped.startsWith(c.dialCode));
      if (match) {
        this.selectedCountry = match;
        this.phoneDigits = stripped.slice(match.dialCode.length);
        this.phoneDisplay = this.formatPhoneDigits(this.phoneDigits);
      }
    }
    this.countrySearchCtrl.setValue(this.selectedCountry, { emitEvent: false });

    this.countrySearchCtrl.valueChanges.subscribe((value) => {
      if (typeof value === 'string') {
        const search = value.replace(/^\+/, '').trim();
        this.filteredCountries = search
          ? PHONE_COUNTRIES.filter(c => c.dialCode.startsWith(search))
          : [...PHONE_COUNTRIES];
      } else {
        this.filteredCountries = [...PHONE_COUNTRIES];
      }
    });
  }

  onCountryFocus(): void {
    this.countrySearchCtrl.setValue('', { emitEvent: true });
  }

  onCountryBlur(): void {
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
    const g1   = digits.slice(0, 2);
    const g2   = digits.slice(2, 5);
    const rest = digits.slice(5);
    const parts: string[] = [g1];
    if (g2) parts.push(g2);
    if (rest) {
      if (rest.length <= 3) {
        parts.push(rest);
      } else {
        for (let i = 0; i < rest.length; i += 2) {
          parts.push(rest.slice(i, i + 2));
        }
      }
    }
    return parts.join(' ');
  }

  get cleanPhoneValue(): string {
    if (!this.phoneDigits) return '';
    return `+${this.selectedCountry.dialCode}${this.phoneDigits}`;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.error = undefined;
    this.isLoading = true;

    this.users.update(this.data.user.id, {
      ...this.form.value,
      phone: this.cleanPhoneValue,
    }).subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        this.error = err?.error?.message ?? 'Update failed.';
        this.isLoading = false;
      },
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  goToChangePassword(): void {
    this.dialogRef.close(false);
    this.router.navigate(['/client/profile/change-password']);
  }
}
