import { Component, Input, HostBinding } from '@angular/core';

export type CardVariant = 'default' | 'elevated' | 'interactive' | 'glass' | 'gradient' | 'outline';

@Component({
  selector: 'app-card',
  standalone: false,
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss'],
})
export class CardComponent {
  @Input() variant: CardVariant = 'default';

  @HostBinding('class') get hostClasses(): string {
    return this.getCardClasses();
  }

  private getCardClasses(): string {
    const variantClass = `card-variant-${this.variant}`;
    return `card-element ${variantClass}`;
  }

  private getVariantClasses(): string {
    const variants: Record<CardVariant, string> = {
      default: 'bg-card border-border shadow-sm',
      elevated: 'bg-card border-border shadow-lg shadow-black/20',
      interactive:
        'bg-card border-border shadow-lg shadow-black/20 hover:-translate-y-1 hover:border-primary/30 hover:shadow-xl hover:shadow-primary/10 cursor-pointer',
      glass: 'bg-white/5 backdrop-blur-xl border-white/10 hover:bg-white/10 hover:border-white/20',
      gradient: 'bg-gradient-to-br from-card to-background border-border',
      outline: 'bg-transparent border-border hover:border-primary/50',
    };

    return variants[this.variant] || variants['default'];
  }
}
