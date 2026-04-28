import { Component, inject, computed } from '@angular/core';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';

@Component({
  selector: 'app-staff-home',
  standalone: false,
  templateUrl: './staff-home.component.html',
  styleUrl: './staff-home.component.scss',
})
export class StaffHomeComponent {
  readonly auth = inject(AuthFacadeService);

  readonly roleLabel = computed(() => {
    if (this.auth.isAdmin()) return 'Admin';
    if (this.auth.isManager()) return 'Manager';
    return 'Employee';
  });

  readonly roleIcon = computed(() => {
    if (this.auth.isAdmin()) return 'admin_panel_settings';
    if (this.auth.isManager()) return 'manage_accounts';
    return 'badge';
  });
}
