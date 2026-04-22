import { Component, inject } from '@angular/core';

import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';

interface FooterLink {
  label: string;
  route: string;
  guestRoute?: string;
}

@Component({
  selector: 'app-site-footer',
  standalone: false,
  templateUrl: './site-footer.component.html',
  styleUrl: './site-footer.component.scss',
})
export class SiteFooterComponent {
  private readonly auth = inject(AuthFacadeService);

  readonly isAuthenticated = this.auth.isAuthenticated;

  currentYear = new Date().getFullYear();

  readonly quickLinks: FooterLink[] = [
    { label: 'Shop Products', route: '/search' },
    { label: 'Book a Service', route: '/book-now' },
    { label: 'Customer Reviews', route: '/reviews' },
    {
      label: 'My Appointments',
      route: '/client/bookings',
      guestRoute: '/auth/login',
    },
  ];

  readonly legalLinks: FooterLink[] = [
    { label: 'Privacy Policy', route: '/privacy-policy' },
    { label: 'Terms of Service', route: '/terms-of-service' },
  ];

  readonly services = [
    'Essential Wash',
    'Interior Detailing',
    'Paint Correction',
    'Ceramic Coating',
  ];

  resolveRoute(link: FooterLink): string {
    if (!this.isAuthenticated() && link.guestRoute) {
      return link.guestRoute;
    }

    return link.route;
  }
}
