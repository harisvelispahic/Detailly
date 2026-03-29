import { Component, Input, HostBinding } from '@angular/core';

export type BadgeVariant =
  | 'default'
  | 'secondary'
  | 'destructive'
  | 'outline'
  | 'success'
  | 'warning'
  | 'info'
  | 'pending'
  | 'confirmed'
  | 'completed'
  | 'cancelled'
  | 'glass';

@Component({
  selector: 'app-badge',
  standalone: false,
  templateUrl: './badge.component.html',
  styleUrls: ['./badge.component.scss'],
})
export class BadgeComponent {
  @Input() variant: BadgeVariant = 'default';

  @HostBinding('class') get hostClasses(): string {
    return this.getBadgeClasses();
  }

  private getBadgeClasses(): string {
    const baseClasses =
      'inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2';

    const variantClasses = this.getVariantClasses();

    return `${baseClasses} ${variantClasses}`;
  }

  private getVariantClasses(): string {
    const variants: Record<BadgeVariant, string> = {
      default: 'border-transparent bg-primary text-primary-foreground hover:bg-primary/80',
      secondary: 'border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80',
      destructive:
        'border-transparent bg-destructive text-destructive-foreground hover:bg-destructive/80',
      outline: 'text-foreground border-border',
      success: 'border-transparent bg-success/20 text-success',
      warning: 'border-transparent bg-warning/20 text-warning',
      info: 'border-transparent bg-info/20 text-info',
      pending: 'border-transparent bg-warning/20 text-warning uppercase tracking-wide',
      confirmed: 'border-transparent bg-info/20 text-info uppercase tracking-wide',
      completed: 'border-transparent bg-success/20 text-success uppercase tracking-wide',
      cancelled: 'border-transparent bg-destructive/20 text-destructive uppercase tracking-wide',
      glass: 'border-white/20 bg-white/10 text-foreground backdrop-blur-sm',
    };

    return variants[this.variant] || variants['default'];
  }
}
