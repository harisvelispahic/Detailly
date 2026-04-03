import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';

interface PublicNavLink {
  route: string;
  label: string;
  icon: string;
  exact?: boolean;
  authOnly?: boolean;
  guestOnly?: boolean;
}

@Component({
  selector: 'app-public-navbar',
  standalone: false,
  templateUrl: './public-navbar.component.html',
  styleUrls: ['./public-navbar.component.scss'],
})
export class PublicNavbarComponent {
  private router = inject(Router);
  private auth = inject(AuthFacadeService);

  readonly currentUser = this.auth.currentUser;
  readonly isAuthenticated = this.auth.isAuthenticated;
  readonly isAdmin = this.auth.isAdmin;
  readonly isManager = this.auth.isManager;
  readonly isEmployee = this.auth.isEmployee;

  readonly navLinks: PublicNavLink[] = [
    { route: '/', label: 'Home', icon: 'home', exact: true },
    { route: '/search', label: 'Shop', icon: 'storefront' },
    { route: '/client/bookings', label: 'Book Now', icon: 'event_available', guestOnly: true },
    { route: '/client/bookings', label: 'My Appointments', icon: 'event_note', authOnly: true },
  ];

  isMenuOpen = false;

  constructor() {
    this.router.events
      .pipe(
        filter((event): event is NavigationEnd => event instanceof NavigationEnd),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.isMenuOpen = false;
      });
  }

  toggleMobileMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMobileMenu(): void {
    this.isMenuOpen = false;
  }

  isLinkVisible(link: PublicNavLink): boolean {
    if (link.authOnly) {
      return this.isAuthenticated();
    }

    if (link.guestOnly) {
      return !this.isAuthenticated();
    }

    return true;
  }

  isLinkActive(link: PublicNavLink): boolean {
    const currentUrl = this.router.url.split('?')[0].split('#')[0];

    if (link.exact) {
      return currentUrl === link.route;
    }

    return currentUrl === link.route || currentUrl.startsWith(`${link.route}/`);
  }

  getAccountRoute(): string {
    if (this.isAdmin()) {
      return '/admin';
    }

    if (this.isManager() || this.isEmployee()) {
      return '/staff';
    }

    if (this.isAuthenticated()) {
      return '/client/bookings';
    }

    return '/auth/login';
  }

  getRoleLabel(): string {
    if (this.isAdmin()) {
      return 'Admin';
    }

    if (this.isManager() || this.isEmployee()) {
      return 'Staff';
    }

    return 'Account';
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => {
        this.closeMobileMenu();
        this.router.navigate(['/']);
      },
      error: () => {
        this.closeMobileMenu();
        this.router.navigate(['/']);
      },
    });
  }
}
