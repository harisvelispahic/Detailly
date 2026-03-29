import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-card-description',
  standalone: false,
  templateUrl: './card-description.component.html',
  styleUrls: ['./card-description.component.scss'],
})
export class CardDescriptionComponent {
  @HostBinding('class') get hostClasses(): string {
    return 'text-sm text-muted-foreground card-description-element';
  }
}
