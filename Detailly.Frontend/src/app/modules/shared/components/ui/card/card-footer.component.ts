import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-card-footer',
  standalone: false,
  templateUrl: './card-footer.component.html',
  styleUrls: ['./card-footer.component.scss'],
})
export class CardFooterComponent {
  @HostBinding('class') get hostClasses(): string {
    return 'card-footer';
  }
}
