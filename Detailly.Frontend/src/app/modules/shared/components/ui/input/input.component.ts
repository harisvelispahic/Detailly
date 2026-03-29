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
    return `flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-base ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50`;
  }
}
