import { Component, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'app-label',
  standalone: false,
  templateUrl: './label.component.html',
  styleUrls: ['./label.component.scss'],
})
export class LabelComponent {
  @Input() for?: string;
  @Input() required: boolean = false;

  @HostBinding('class') get hostClasses(): string {
    return 'label-element';
  }
}
