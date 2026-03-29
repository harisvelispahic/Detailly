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
    return `flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-base ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 resize-none`;
  }
}
