import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddressesApiService } from '../../../../api-services/addresses/addresses-api.service';
import { ListMyAddressesQueryDto } from '../../../../api-services/addresses/addresses-api.model';

export interface CountryOption {
  name: string;
  iso2: string;
}

export const ALL_COUNTRIES: CountryOption[] = [
  { name: 'Afghanistan', iso2: 'af' }, { name: 'Albania', iso2: 'al' },
  { name: 'Algeria', iso2: 'dz' }, { name: 'Andorra', iso2: 'ad' },
  { name: 'Angola', iso2: 'ao' }, { name: 'Argentina', iso2: 'ar' },
  { name: 'Armenia', iso2: 'am' }, { name: 'Australia', iso2: 'au' },
  { name: 'Austria', iso2: 'at' }, { name: 'Azerbaijan', iso2: 'az' },
  { name: 'Bahrain', iso2: 'bh' }, { name: 'Bangladesh', iso2: 'bd' },
  { name: 'Belarus', iso2: 'by' }, { name: 'Belgium', iso2: 'be' },
  { name: 'Belize', iso2: 'bz' }, { name: 'Bolivia', iso2: 'bo' },
  { name: 'Bosnia and Herzegovina', iso2: 'ba' }, { name: 'Brazil', iso2: 'br' },
  { name: 'Bulgaria', iso2: 'bg' }, { name: 'Cambodia', iso2: 'kh' },
  { name: 'Canada', iso2: 'ca' }, { name: 'Chile', iso2: 'cl' },
  { name: 'China', iso2: 'cn' }, { name: 'Colombia', iso2: 'co' },
  { name: 'Costa Rica', iso2: 'cr' }, { name: 'Croatia', iso2: 'hr' },
  { name: 'Cuba', iso2: 'cu' }, { name: 'Cyprus', iso2: 'cy' },
  { name: 'Czech Republic', iso2: 'cz' }, { name: 'Denmark', iso2: 'dk' },
  { name: 'Dominican Republic', iso2: 'do' }, { name: 'Ecuador', iso2: 'ec' },
  { name: 'Egypt', iso2: 'eg' }, { name: 'Estonia', iso2: 'ee' },
  { name: 'Ethiopia', iso2: 'et' }, { name: 'Finland', iso2: 'fi' },
  { name: 'France', iso2: 'fr' }, { name: 'Georgia', iso2: 'ge' },
  { name: 'Germany', iso2: 'de' }, { name: 'Ghana', iso2: 'gh' },
  { name: 'Greece', iso2: 'gr' }, { name: 'Guatemala', iso2: 'gt' },
  { name: 'Honduras', iso2: 'hn' }, { name: 'Hungary', iso2: 'hu' },
  { name: 'Iceland', iso2: 'is' }, { name: 'India', iso2: 'in' },
  { name: 'Indonesia', iso2: 'id' }, { name: 'Iran', iso2: 'ir' },
  { name: 'Iraq', iso2: 'iq' }, { name: 'Ireland', iso2: 'ie' },
  { name: 'Israel', iso2: 'il' }, { name: 'Italy', iso2: 'it' },
  { name: 'Jamaica', iso2: 'jm' }, { name: 'Japan', iso2: 'jp' },
  { name: 'Jordan', iso2: 'jo' }, { name: 'Kazakhstan', iso2: 'kz' },
  { name: 'Kenya', iso2: 'ke' }, { name: 'Kosovo', iso2: 'xk' },
  { name: 'Kuwait', iso2: 'kw' }, { name: 'Latvia', iso2: 'lv' },
  { name: 'Lebanon', iso2: 'lb' }, { name: 'Libya', iso2: 'ly' },
  { name: 'Lithuania', iso2: 'lt' }, { name: 'Luxembourg', iso2: 'lu' },
  { name: 'Malaysia', iso2: 'my' }, { name: 'Malta', iso2: 'mt' },
  { name: 'Mexico', iso2: 'mx' }, { name: 'Moldova', iso2: 'md' },
  { name: 'Montenegro', iso2: 'me' }, { name: 'Morocco', iso2: 'ma' },
  { name: 'Netherlands', iso2: 'nl' }, { name: 'New Zealand', iso2: 'nz' },
  { name: 'Nigeria', iso2: 'ng' }, { name: 'North Macedonia', iso2: 'mk' },
  { name: 'Norway', iso2: 'no' }, { name: 'Oman', iso2: 'om' },
  { name: 'Pakistan', iso2: 'pk' }, { name: 'Palestine', iso2: 'ps' },
  { name: 'Panama', iso2: 'pa' }, { name: 'Paraguay', iso2: 'py' },
  { name: 'Peru', iso2: 'pe' }, { name: 'Philippines', iso2: 'ph' },
  { name: 'Poland', iso2: 'pl' }, { name: 'Portugal', iso2: 'pt' },
  { name: 'Qatar', iso2: 'qa' }, { name: 'Romania', iso2: 'ro' },
  { name: 'Russia', iso2: 'ru' }, { name: 'Saudi Arabia', iso2: 'sa' },
  { name: 'Serbia', iso2: 'rs' }, { name: 'Singapore', iso2: 'sg' },
  { name: 'Slovakia', iso2: 'sk' }, { name: 'Slovenia', iso2: 'si' },
  { name: 'Somalia', iso2: 'so' }, { name: 'South Africa', iso2: 'za' },
  { name: 'South Korea', iso2: 'kr' }, { name: 'Spain', iso2: 'es' },
  { name: 'Sudan', iso2: 'sd' }, { name: 'Sweden', iso2: 'se' },
  { name: 'Switzerland', iso2: 'ch' }, { name: 'Syria', iso2: 'sy' },
  { name: 'Taiwan', iso2: 'tw' }, { name: 'Thailand', iso2: 'th' },
  { name: 'Tunisia', iso2: 'tn' }, { name: 'Turkey', iso2: 'tr' },
  { name: 'Ukraine', iso2: 'ua' }, { name: 'United Arab Emirates', iso2: 'ae' },
  { name: 'United Kingdom', iso2: 'gb' }, { name: 'United States', iso2: 'us' },
  { name: 'Uruguay', iso2: 'uy' }, { name: 'Uzbekistan', iso2: 'uz' },
  { name: 'Venezuela', iso2: 've' }, { name: 'Vietnam', iso2: 'vn' },
  { name: 'Yemen', iso2: 'ye' }, { name: 'Zimbabwe', iso2: 'zw' },
];

export interface AddressDialogData {
  address?: ListMyAddressesQueryDto;
}

@Component({
  selector: 'app-address-dialog',
  standalone: false,
  templateUrl: './address-dialog.component.html',
  styleUrl: './address-dialog.component.scss',
})
export class AddressDialogComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  error?: string;

  countrySearchCtrl = new FormControl<CountryOption | string>('');
  filteredCountries: CountryOption[] = [...ALL_COUNTRIES];

  get isEdit(): boolean {
    return !!this.data.address;
  }

  displayCountry = (value: CountryOption | string | null): string => {
    if (!value) return '';
    if (typeof value === 'string') return value;
    return value.name;
  };

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddressDialogComponent>,
    private addresses: AddressesApiService,
    @Inject(MAT_DIALOG_DATA) public data: AddressDialogData,
  ) {}

  ngOnInit(): void {
    const a = this.data.address;
    this.form = this.fb.group({
      street: [a?.street ?? '', Validators.required],
      city: [a?.city ?? '', Validators.required],
      postalCode: [a?.postalCode ?? '', Validators.required],
      region: [a?.region ?? ''],
      country: [a?.country ?? '', Validators.required],
    });

    if (a?.country) {
      const match = ALL_COUNTRIES.find(c => c.name === a.country);
      this.countrySearchCtrl.setValue(match ?? a.country);
    }

    this.countrySearchCtrl.valueChanges.subscribe((value) => {
      if (typeof value === 'string') {
        const search = value.trim().toLowerCase();
        this.filteredCountries = search
          ? ALL_COUNTRIES.filter(c => c.name.toLowerCase().includes(search))
          : [...ALL_COUNTRIES];
      } else {
        this.filteredCountries = [...ALL_COUNTRIES];
      }
    });
  }

  onCountryFocus(): void {
    this.countrySearchCtrl.setValue('', { emitEvent: true });
  }

  onCountryBlur(): void {
    setTimeout(() => {
      if (typeof this.countrySearchCtrl.value !== 'object') {
        const current = this.form.get('country')?.value;
        const match = ALL_COUNTRIES.find(c => c.name === current);
        this.countrySearchCtrl.setValue(match ?? current ?? '', { emitEvent: false });
      }
    }, 250);
  }

  onCountrySelected(option: CountryOption): void {
    this.form.patchValue({ country: option.name });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.error = undefined;
    this.isLoading = true;
    const value = this.form.value;

    if (this.isEdit) {
      this.addresses.update(this.data.address!.id, value).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => { this.error = err?.error?.message ?? 'Update failed.'; this.isLoading = false; },
      });
    } else {
      this.addresses.create(value).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => { this.error = err?.error?.message ?? 'Create failed.'; this.isLoading = false; },
      });
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
