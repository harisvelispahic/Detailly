import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

interface PlaceholderPageData {
  eyebrow?: string;
  title: string;
  accent?: string;
  description: string;
  highlights?: string[];
  primaryLabel?: string;
  primaryRoute?: string;
  secondaryLabel?: string;
  secondaryRoute?: string;
}

@Component({
  selector: 'app-public-placeholder-page',
  standalone: false,
  templateUrl: './public-placeholder-page.component.html',
  styleUrl: './public-placeholder-page.component.scss',
})
export class PublicPlaceholderPageComponent {
  private readonly route = inject(ActivatedRoute);

  readonly page = this.route.snapshot.data as PlaceholderPageData;
}
