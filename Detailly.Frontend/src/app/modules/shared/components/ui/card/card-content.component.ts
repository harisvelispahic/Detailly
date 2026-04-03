import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-card-content',
  standalone: false,
  templateUrl: './card-content.component.html',
  styleUrls: ['./card-content.component.scss'],
})
export class CardContentComponent {
  @HostBinding('class') get hostClasses(): string {
    return 'card-content';
  }
}
