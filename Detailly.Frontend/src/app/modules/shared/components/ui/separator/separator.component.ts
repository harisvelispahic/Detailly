import { Component, Input, HostBinding } from '@angular/core';

export type SeparatorOrientation = 'horizontal' | 'vertical';

@Component({
  selector: 'app-separator',
  standalone: false,
  templateUrl: './separator.component.html',
  styleUrls: ['./separator.component.scss'],
})
export class SeparatorComponent {
  @Input() orientation: SeparatorOrientation = 'horizontal';
  @Input() decorative: boolean = true;

  @HostBinding('class') get hostClasses(): string {
    return this.getSeparatorClasses();
  }

  @HostBinding('attr.role') get role(): string | null {
    return this.decorative ? null : 'separator';
  }

  @HostBinding('attr.aria-orientation') get ariaOrientation(): string {
    return this.orientation;
  }

  private getSeparatorClasses(): string {
    const baseClasses = 'separator-element';
    const orientationClasses =
      this.orientation === 'horizontal' ? 'separator-horizontal' : 'separator-vertical';
    return `${baseClasses} ${orientationClasses}`;
  }
}
