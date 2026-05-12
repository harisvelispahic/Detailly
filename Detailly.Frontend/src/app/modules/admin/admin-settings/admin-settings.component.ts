import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SystemSettingsApiService } from '../../../api-services/system-settings/system-settings-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-admin-settings',
  standalone: false,
  templateUrl: './admin-settings.component.html',
  styleUrl: './admin-settings.component.scss',
})
export class AdminSettingsComponent implements OnInit {
  private settingsApi = inject(SystemSettingsApiService);
  private toaster = inject(ToasterService);
  private fb = inject(FormBuilder);

  form!: FormGroup;
  loading = true;
  saving = false;

  ngOnInit(): void {
    this.form = this.fb.group({
      standardWalletBonusPercent: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
      fleetWalletBonusPercent: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
      reviewWindowDays: [null, [Validators.required, Validators.min(1), Validators.max(90)]],
      baseFleetDiscountPercent: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
      perVehicleFleetDiscountPercent: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
      maxFleetDiscountPercent: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
    });

    this.settingsApi.get().subscribe({
      next: (s) => {
        this.form.patchValue(s);
        this.loading = false;
      },
      error: () => {
        this.toaster.error('Failed to load settings.');
        this.loading = false;
      },
    });
  }

  save(): void {
    if (this.form.invalid || this.saving) return;
    this.saving = true;

    this.settingsApi.update(this.form.value).subscribe({
      next: () => {
        this.toaster.success('Settings saved successfully.');
        this.saving = false;
      },
      error: () => {
        this.toaster.error('Failed to save settings.');
        this.saving = false;
      },
    });
  }
}
