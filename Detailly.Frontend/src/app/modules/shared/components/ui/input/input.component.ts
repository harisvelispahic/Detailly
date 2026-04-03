import { Component, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'app-input',
  standalone: false,
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
})
export class InputComponent {
  @Input() type: string = 'text';
  @Input() placeholder: string = '';
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() id?: string;
  @Input() name?: string;
  @Input() value: string = '';
  @Input() minLength?: number;
  @Input() maxLength?: number;
  @Input() pattern?: string;

  @HostBinding('class') get hostClasses(): string {
    return 'input-wrapper';
  }

  getInputClasses(): string {
    return 'input-element';
  }
}
