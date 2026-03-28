import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { PricingTier } from '../../../../data/landing-detailing.model';

@Component({
  selector: 'app-pricing',
  templateUrl: './pricing.component.html',
  styleUrl: './pricing.component.scss',
  standalone: false,
})
export class PricingComponent {
  @Input() pricingTiers: PricingTier[] = [];

  constructor(private router: Router) {}

  bookNow(): void {
    this.router.navigate(['/booking']);
  }
}
