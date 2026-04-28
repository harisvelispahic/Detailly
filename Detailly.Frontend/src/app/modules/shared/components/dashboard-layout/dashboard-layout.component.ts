import { Component, computed, inject, signal } from '@angular/core';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';

@Component({
  selector: 'app-dashboard-layout',
  standalone: false,
  templateUrl: './dashboard-layout.component.html',
  styleUrl: './dashboard-layout.component.scss',
})
export class DashboardLayoutComponent {
  readonly auth = inject(AuthFacadeService);

  readonly isSidebarOpen = signal(false);

  readonly initials = computed(() => {
    const email = this.auth.currentUser()?.email ?? '';
    const local = email.split('@')[0];
    const parts = local.split(/[._\-]/);
    if (parts.length >= 2 && parts[1]?.length) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    }
    return local.slice(0, 2).toUpperCase();
  });

  toggleSidebar(): void {
    this.isSidebarOpen.update((v) => !v);
  }

  closeSidebar(): void {
    this.isSidebarOpen.set(false);
  }
}
