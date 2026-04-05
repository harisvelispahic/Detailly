import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';

interface SiteNavLink {
  label: string;
  icon: string;
  route: string;
  exact?: boolean;
  guestRoute?: string;
}

@Component({
  selector: 'app-site-navbar',
  standalone: false,
  templateUrl: './site-navbar.component.html',
  styleUrl: './site-navbar.component.scss',
})
export class SiteNavbarComponent {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthFacadeService);

  readonly currentUser = this.auth.currentUser;
  readonly isAuthenticated = this.auth.isAuthenticated;
  readonly isAdmin = this.auth.isAdmin;
  readonly isManager = this.auth.isManager;
  readonly isEmployee = this.auth.isEmployee;

  readonly navLinks: SiteNavLink[] = [
    { label: 'Home', icon: 'home', route: '/', exact: true },
    { label: 'Shop', icon: 'storefront', route: '/search' },
    { label: 'Book Now', icon: 'calendar_month', route: '/book-now' },
    {
      label: 'My Appointments',
      icon: 'event_note',
      route: '/client/bookings',
      guestRoute: '/auth/login',
    },
    { label: 'Reviews', icon: 'reviews', route: '/reviews' },
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

  resolveRoute(link: SiteNavLink): string {
    if (!this.isAuthenticated() && link.guestRoute) {
      return link.guestRoute;
    }

    return link.route;
  }

  isLinkActive(link: SiteNavLink): boolean {
    const currentUrl = this.router.url.split('?')[0].split('#')[0];
    const route = this.resolveRoute(link);

    if (link.exact) {
      return currentUrl === route;
    }

    return currentUrl === route || currentUrl.startsWith(`${route}/`);
  }

  isShopActive(): boolean {
    const currentUrl = this.router.url.split('?')[0].split('#')[0];
    return currentUrl === '/search' || currentUrl.startsWith('/search/');
  }

  isAccountActive(): boolean {
    const currentUrl = this.router.url.split('?')[0].split('#')[0];
    const accountRoute = this.getAccountRoute();

    return currentUrl === accountRoute || currentUrl.startsWith(`${accountRoute}/`);
  }

  getAccountRoute(): string {
    if (this.isAdmin()) {
      return '/admin';
    }

    if (this.isManager() || this.isEmployee()) {
      return '/staff';
    }

    if (this.isAuthenticated()) {
      return '/client/profile';
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
