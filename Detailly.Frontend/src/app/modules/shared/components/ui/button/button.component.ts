import { Component, Input, Output, EventEmitter, HostBinding } from '@angular/core';

export type ButtonVariant =
  | 'default'
  | 'destructive'
  | 'outline'
  | 'secondary'
  | 'ghost'
  | 'link'
  | 'gradient'
  | 'hero'
  | 'hero-outline'
  | 'glass'
  | 'success';

export type ButtonSize = 'default' | 'sm' | 'lg' | 'xl' | 'icon';

@Component({
  selector: 'app-button',
  standalone: false,
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
})
export class ButtonComponent {
  @Input() variant: ButtonVariant = 'default';
  @Input() size: ButtonSize = 'default';
  @Input() disabled: boolean = false;
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() ariaLabel?: string;

  @Output() clicked = new EventEmitter<MouseEvent>();

  @HostBinding('class') get hostClasses(): string {
    return 'app-button-wrapper';
  }

  onClickHandler(event: MouseEvent): void {
    if (!this.disabled) {
      this.clicked.emit(event);
    }
  }

  getButtonClasses(): string {
    const variantClass = `btn-variant-${this.variant}`;
    const sizeClass = `btn-size-${this.size}`;
    return `button-element ${variantClass} ${sizeClass}`;
  }
}
