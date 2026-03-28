import { Component, Input } from '@angular/core';
import { LandingStats } from '../../../../data/landing-detailing.model';

@Component({
  selector: 'app-hero',
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.scss',
  standalone: false,
})
export class HeroComponent {
  @Input() stats!: LandingStats;
}
