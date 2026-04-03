import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-card-title',
  standalone: false,
  templateUrl: './card-title.component.html',
  styleUrls: ['./card-title.component.scss'],
})
export class CardTitleComponent {
  @HostBinding('class') get hostClasses(): string {
    return 'card-title';
  }
}
