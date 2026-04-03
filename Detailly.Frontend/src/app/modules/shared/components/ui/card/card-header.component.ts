import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-card-header',
  standalone: false,
  templateUrl: './card-header.component.html',
  styleUrls: ['./card-header.component.scss'],
})
export class CardHeaderComponent {
  @HostBinding('class') get hostClasses(): string {
    return 'card-header';
  }
}
