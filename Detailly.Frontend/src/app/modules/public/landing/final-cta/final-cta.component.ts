import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-final-cta',
  templateUrl: './final-cta.component.html',
  styleUrl: './final-cta.component.scss',
  standalone: false,
})
export class FinalCtaComponent {
  constructor(private router: Router) {}

  bookNow(): void {
    this.router.navigate(['/booking']);
  }

  exploreProducts(): void {
    this.router.navigate(['/shop']);
  }
}
