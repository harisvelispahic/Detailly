import { Component, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'app-textarea',
  standalone: false,
  templateUrl: './textarea.component.html',
  styleUrls: ['./textarea.component.scss'],
})
export class TextareaComponent {
  @Input() placeholder: string = '';
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() id?: string;
  @Input() name?: string;
  @Input() value: string = '';
  @Input() minLength?: number;
  @Input() maxLength?: number;
  @Input() rows: number = 4;

  @HostBinding('class') get hostClasses(): string {
    return 'textarea-wrapper';
  }

  getTextareaClasses(): string {
    return 'textarea-element';
  }
}
