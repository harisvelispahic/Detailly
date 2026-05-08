import { Injectable, inject, computed } from '@angular/core';
import { AuthFacadeService } from './auth-facade.service';

@Injectable({ providedIn: 'root' })
export class CurrentUserService {
  private auth = inject(AuthFacadeService);

  /** Read-only signal for the UI. */
  currentUser = computed(() => this.auth.currentUser());

  isAuthenticated = computed(() => this.auth.isAuthenticated());
  isAdmin = computed(() => this.auth.isAdmin());
  isManager = computed(() => this.auth.isManager());
  isEmployee = computed(() => this.auth.isEmployee());
  isFleet = computed(() => this.auth.isFleet());
  isStandard = computed(() => this.auth.isStandard());

  get snapshot() {
    return this.auth.currentUser();
  }

  /** Priority: admin > staff > client */
  getDefaultRoute(): string {
    const user = this.snapshot;
    if (!user) return '/login';

    if (user.isAdmin) return '/staff';

    // staff
    if (user.isManager || user.isEmployee) return '/staff';

    // clients
    return '/client';
  }
}
