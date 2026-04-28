import { Component, inject, Inject, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import {
  CreateStaffMemberCommand,
  ListStaffMembersQueryDto,
  UpdateStaffMemberCommand,
} from '../../../../../api-services/staff-members/staff-members-api.models';
import { StaffMembersApiService } from '../../../../../api-services/staff-members/staff-members-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';

export interface PhoneCountry {
  name: string;
  iso2: string;
  dialCode: string;
}

export interface StaffMemberUpsertDialogData {
  member: ListStaffMembersQueryDto | null;
  isAdmin: boolean;
}

function passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
  const value: string = control.value ?? '';
  if (!value) return null;
  if (
    !/[A-Z]/.test(value) ||
    !/[a-z]/.test(value) ||
    !/[0-9]/.test(value) ||
    !/[^A-Za-z0-9]/.test(value)
  ) {
    return { passwordStrength: true };
  }
  return null;
}

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password');
  const confirm = control.get('confirmPassword');
  if (!password || !confirm) return null;
  return password.value !== confirm.value ? { passwordMismatch: true } : null;
}

const PHONE_COUNTRIES: PhoneCountry[] = [
  { name: 'United States',        iso2: 'us', dialCode: '1'   },
  { name: 'Canada',               iso2: 'ca', dialCode: '1'   },
  { name: 'Russia',               iso2: 'ru', dialCode: '7'   },
  { name: 'South Africa',         iso2: 'za', dialCode: '27'  },
  { name: 'Greece',               iso2: 'gr', dialCode: '30'  },
  { name: 'Netherlands',          iso2: 'nl', dialCode: '31'  },
  { name: 'Belgium',              iso2: 'be', dialCode: '32'  },
  { name: 'France',               iso2: 'fr', dialCode: '33'  },
  { name: 'Spain',                iso2: 'es', dialCode: '34'  },
  { name: 'Hungary',              iso2: 'hu', dialCode: '36'  },
  { name: 'Italy',                iso2: 'it', dialCode: '39'  },
  { name: 'Romania',              iso2: 'ro', dialCode: '40'  },
  { name: 'Switzerland',          iso2: 'ch', dialCode: '41'  },
  { name: 'Austria',              iso2: 'at', dialCode: '43'  },
  { name: 'United Kingdom',       iso2: 'gb', dialCode: '44'  },
  { name: 'Denmark',              iso2: 'dk', dialCode: '45'  },
  { name: 'Sweden',               iso2: 'se', dialCode: '46'  },
  { name: 'Norway',               iso2: 'no', dialCode: '47'  },
  { name: 'Poland',               iso2: 'pl', dialCode: '48'  },
  { name: 'Germany',              iso2: 'de', dialCode: '49'  },
  { name: 'Mexico',               iso2: 'mx', dialCode: '52'  },
  { name: 'Brazil',               iso2: 'br', dialCode: '55'  },
  { name: 'Australia',            iso2: 'au', dialCode: '61'  },
  { name: 'Japan',                iso2: 'jp', dialCode: '81'  },
  { name: 'China',                iso2: 'cn', dialCode: '86'  },
  { name: 'Turkey',               iso2: 'tr', dialCode: '90'  },
  { name: 'India',                iso2: 'in', dialCode: '91'  },
  { name: 'Albania',              iso2: 'al', dialCode: '355' },
  { name: 'Bulgaria',             iso2: 'bg', dialCode: '359' },
  { name: 'Ukraine',              iso2: 'ua', dialCode: '380' },
  { name: 'Serbia',               iso2: 'rs', dialCode: '381' },
  { name: 'Montenegro',           iso2: 'me', dialCode: '382' },
  { name: 'Kosovo',               iso2: 'xk', dialCode: '383' },
  { name: 'Croatia',              iso2: 'hr', dialCode: '385' },
  { name: 'Slovenia',             iso2: 'si', dialCode: '386' },
  { name: 'Bosnia & Herzegovina', iso2: 'ba', dialCode: '387' },
  { name: 'North Macedonia',      iso2: 'mk', dialCode: '389' },
  { name: 'Czech Republic',       iso2: 'cz', dialCode: '420' },
  { name: 'Saudi Arabia',         iso2: 'sa', dialCode: '966' },
  { name: 'UAE',                  iso2: 'ae', dialCode: '971' },
];

@Component({
  selector: 'app-staff-member-upsert-dialog',
  standalone: false,
  templateUrl: './staff-member-upsert-dialog.component.html',
  styleUrl: './staff-member-upsert-dialog.component.scss',
})
export class StaffMemberUpsertDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private staffApi = inject(StaffMembersApiService);
  private toaster = inject(ToasterService);
  private dialogRef = inject(MatDialogRef<StaffMemberUpsertDialogComponent>);

  form!: FormGroup;
  isSubmitting = false;
  hidePassword = true;
  hideConfirmPassword = true;

  readonly phoneCountries = PHONE_COUNTRIES;
  filteredCountries: PhoneCountry[] = [...PHONE_COUNTRIES];
  selectedCountry: PhoneCountry = PHONE_COUNTRIES.find((c) => c.dialCode === '387')!;
  countrySearchCtrl = new FormControl<PhoneCountry | string>(this.selectedCountry);
  phoneDigits = '';
  phoneDisplay = '';

  get isEditMode(): boolean {
    return this.data.member !== null;
  }

  get passwordMismatch(): boolean {
    const ctrl = this.form?.get('confirmPassword');
    return !!this.form?.hasError('passwordMismatch') && !!(ctrl?.dirty || ctrl?.touched);
  }

  displayCountry = (value: PhoneCountry | string | null): string => {
    if (!value) return '';
    if (typeof value === 'string') return value;
    return `+${value.dialCode}`;
  };

  constructor(@Inject(MAT_DIALOG_DATA) public data: StaffMemberUpsertDialogData) {}

  ngOnInit(): void {
    const m = this.data.member;

    this.form = this.fb.group(
      {
        firstName: [m?.firstName ?? '', [Validators.required, Validators.maxLength(100)]],
        lastName: [m?.lastName ?? '', [Validators.required, Validators.maxLength(100)]],
        username: [m?.username ?? '', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
        email: [m?.email ?? '', [Validators.required, Validators.email]],
        ...(this.isEditMode
          ? {}
          : {
              password: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
              confirmPassword: ['', [Validators.required]],
            }),
        isManager: [{ value: m?.isManager ?? false, disabled: !this.data.isAdmin }],
      },
      this.isEditMode ? {} : { validators: passwordMatchValidator },
    );

    // Pre-fill phone if editing
    if (m?.phone) {
      this.parseExistingPhone(m.phone);
    }

    this.countrySearchCtrl.valueChanges.subscribe((value) => {
      if (typeof value === 'string') {
        const search = value.replace(/^\+/, '').trim();
        this.filteredCountries = search
          ? this.phoneCountries.filter((c) => c.dialCode.startsWith(search))
          : [...this.phoneCountries];
      } else {
        this.filteredCountries = [...this.phoneCountries];
      }
    });
  }

  private parseExistingPhone(phone: string): void {
    const sorted = [...this.phoneCountries].sort((a, b) => b.dialCode.length - a.dialCode.length);
    const stripped = phone.replace(/^\+/, '');
    const match = sorted.find((c) => stripped.startsWith(c.dialCode));
    if (match) {
      this.selectedCountry = match;
      this.countrySearchCtrl.setValue(match, { emitEvent: false });
      this.phoneDigits = stripped.slice(match.dialCode.length);
      this.phoneDisplay = this.formatPhoneDigits(this.phoneDigits);
    } else {
      this.phoneDigits = stripped;
      this.phoneDisplay = this.formatPhoneDigits(stripped);
    }
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
    const g1 = digits.slice(0, 2);
    const g2 = digits.slice(2, 5);
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

  get cleanPhone(): string | null {
    if (!this.phoneDigits) return null;
    return `+${this.selectedCountry.dialCode}${this.phoneDigits}`;
  }

  onSave(): void {
    if (this.form.invalid || this.isSubmitting) return;
    this.isSubmitting = true;

    const v = this.form.getRawValue();

    if (this.isEditMode) {
      const command: UpdateStaffMemberCommand = {
        firstName: v.firstName.trim(),
        lastName: v.lastName.trim(),
        username: v.username.trim(),
        email: v.email.trim().toLowerCase(),
        phone: this.cleanPhone,
      };
      this.staffApi.update(this.data.member!.id, command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.toaster.error(this.extractError(err) ?? 'Failed to update staff member.');
        },
      });
    } else {
      const command: CreateStaffMemberCommand = {
        firstName: v.firstName.trim(),
        lastName: v.lastName.trim(),
        username: v.username.trim(),
        email: v.email.trim().toLowerCase(),
        password: v.password,
        phone: this.cleanPhone,
        isManager: v.isManager ?? false,
      };
      this.staffApi.create(command).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.isSubmitting = false;
          this.toaster.error(this.extractError(err) ?? 'Failed to create staff member.');
        },
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private extractError(err: any): string | null {
    return err?.error?.message ?? err?.error?.title ?? err?.message ?? null;
  }
}
