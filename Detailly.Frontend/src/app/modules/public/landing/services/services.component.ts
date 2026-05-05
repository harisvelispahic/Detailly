import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import {
  GetAvailableAddonsQueryDto,
  ListServicePackagesQueryDto,
} from '../../../../api-services/service-packages/service-packages-api.models';

export interface ServicePackageWithAddons extends ListServicePackagesQueryDto {
  availableAddons: GetAvailableAddonsQueryDto[];
}

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrl: './services.component.scss',
  standalone: false,
})
export class ServicesComponent {
  @Input() services: ServicePackageWithAddons[] = [];

  constructor(private router: Router) {}

  bookNow(serviceId: number): void {
    this.router.navigate(['/book-now'], { queryParams: { packageId: serviceId } });
  }

  formatDuration(minutes: number): string {
    if (minutes < 60) return `${minutes} min`;
    const hours = Math.floor(minutes / 60);
    const remainder = minutes % 60;
    return remainder > 0 ? `${hours}h ${remainder}min` : `${hours}h`;
  }

  isFeatured(index: number): boolean {
    return this.services.length === 3 && index === 1;
  }
}
