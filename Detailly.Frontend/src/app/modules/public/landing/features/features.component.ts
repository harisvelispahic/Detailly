import { Component, Input } from '@angular/core';

export interface Feature {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-features',
  templateUrl: './features.component.html',
  styleUrl: './features.component.scss',
  standalone: false,
})
export class FeaturesComponent {
  @Input() features: Feature[] = [];
}
