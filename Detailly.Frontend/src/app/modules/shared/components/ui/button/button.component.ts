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
    const baseClasses =
      'inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-lg text-sm font-medium transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed';

    const variantClasses = this.getVariantClasses();
    const sizeClasses = this.getSizeClasses();

    return `${baseClasses} ${variantClasses} ${sizeClasses}`;
  }

  private getVariantClasses(): string {
    const variants: Record<ButtonVariant, string> = {
      default:
        'bg-primary text-primary-foreground shadow hover:bg-primary/90 hover:shadow-lg hover:shadow-primary/20',
      destructive: 'bg-destructive text-destructive-foreground shadow-sm hover:bg-destructive/90',
      outline:
        'border border-border bg-transparent text-foreground hover:bg-secondary hover:border-primary/50',
      secondary: 'bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80',
      ghost: 'text-foreground hover:bg-secondary hover:text-foreground',
      link: 'text-primary underline-offset-4 hover:underline',
      gradient:
        'bg-gradient-to-r from-purple-600 via-violet-600 to-indigo-600 text-white shadow-lg hover:opacity-90 hover:scale-[1.02] hover:shadow-xl hover:shadow-primary/30',
      hero: 'bg-gradient-to-r from-purple-600 via-violet-600 to-indigo-600 text-white font-semibold shadow-lg shadow-primary/30 hover:scale-[1.02] hover:shadow-xl hover:shadow-primary/40 transition-all duration-300',
      'hero-outline':
        'border-2 border-primary/50 bg-transparent text-foreground hover:bg-primary/10 hover:border-primary transition-all duration-300',
      glass:
        'bg-white/5 backdrop-blur-md border border-white/10 text-foreground hover:bg-white/10 hover:border-white/20',
      success: 'bg-success text-success-foreground shadow-sm hover:bg-success/90',
    };

    return variants[this.variant] || variants['default'];
  }

  private getSizeClasses(): string {
    const sizes: Record<ButtonSize, string> = {
      default: 'h-10 px-4 py-2',
      sm: 'h-9 rounded-md px-3 text-xs',
      lg: 'h-12 rounded-lg px-8 text-base',
      xl: 'h-14 rounded-xl px-10 text-lg',
      icon: 'h-10 w-10',
    };

    return sizes[this.size] || sizes['default'];
  }
}
