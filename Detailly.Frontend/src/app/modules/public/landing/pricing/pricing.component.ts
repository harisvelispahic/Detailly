import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

export interface PricingTier {
  name: string;
  price: number;
  description: string;
  features: string[];
  popular: boolean;
}

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
    this.router.navigate(['/book-now']);
  }
}
