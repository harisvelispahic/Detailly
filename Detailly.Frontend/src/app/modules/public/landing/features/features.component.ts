import { Component, Input } from '@angular/core';
import { Feature } from '../../../../data/landing-detailing.model';

@Component({
  selector: 'app-features',
  templateUrl: './features.component.html',
  styleUrl: './features.component.scss',
  standalone: false,
})
export class FeaturesComponent {
  @Input() features: Feature[] = [];
}
