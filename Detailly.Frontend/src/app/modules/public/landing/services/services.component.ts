import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Service } from '../../../../data/landing-detailing.model';

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrl: './services.component.scss',
  standalone: false,
})
export class ServicesComponent {
  @Input() services: Service[] = [];

  constructor(private router: Router) {}

  viewAllServices(): void {
    this.router.navigate(['/booking']);
  }
}
