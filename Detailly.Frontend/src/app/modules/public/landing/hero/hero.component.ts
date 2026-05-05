import { Component, Input } from '@angular/core';

export interface LandingStats {
  carsDetailed: string;
  averageRating: string;
  yearsExperience: string;
}

@Component({
  selector: 'app-hero',
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.scss',
  standalone: false,
})
export class HeroComponent {
  @Input() stats!: LandingStats;
}
