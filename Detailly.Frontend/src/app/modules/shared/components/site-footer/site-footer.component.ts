import { Component, inject, OnInit } from '@angular/core';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { ServicePackagesApiService } from '../../../../api-services/service-packages/service-packages-api.service';
import {
  ListServicePackagesQueryDto,
  ListServicePackagesRequest,
} from '../../../../api-services/service-packages/service-packages-api.models';

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
export class SiteFooterComponent implements OnInit {
  private readonly auth = inject(AuthFacadeService);
  private readonly servicePackagesApi = inject(ServicePackagesApiService);

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

  footerServices: ListServicePackagesQueryDto[] = [];

  ngOnInit(): void {
    const req = new ListServicePackagesRequest();
    req.paging.pageSize = 4;
    this.servicePackagesApi.list(req).subscribe(result => {
      this.footerServices = result.items;
    });
  }

  resolveRoute(link: FooterLink): string {
    if (!this.isAuthenticated() && link.guestRoute) {
      return link.guestRoute;
    }

    return link.route;
  }
}
