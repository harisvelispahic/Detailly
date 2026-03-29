import { Component, Input, HostBinding } from '@angular/core';

export type ContainerSize = 'sm' | 'md' | 'lg' | 'xl' | 'full';

@Component({
  selector: 'app-container',
  standalone: false,
  templateUrl: './container.component.html',
  styleUrls: ['./container.component.scss'],
})
export class ContainerComponent {
  @Input() size: ContainerSize = 'lg';
  @Input() padding: boolean = true;

  @HostBinding('class') get hostClasses(): string {
    return this.getContainerClasses();
  }

  private getContainerClasses(): string {
    const baseClasses = 'mx-auto';

    const sizeClasses = this.getSizeClasses();
    const paddingClasses = this.padding ? 'px-4 md:px-6' : '';

    return `${baseClasses} ${sizeClasses} ${paddingClasses} container-element`;
  }

  private getSizeClasses(): string {
    const sizes: Record<ContainerSize, string> = {
      sm: 'max-w-sm',
      md: 'max-w-md',
      lg: 'max-w-4xl',
      xl: 'max-w-6xl',
      full: 'w-full',
    };

    return sizes[this.size] || sizes['lg'];
  }
}
