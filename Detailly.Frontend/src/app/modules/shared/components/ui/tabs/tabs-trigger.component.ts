import { Component, Input, Output, EventEmitter, HostBinding } from '@angular/core';

@Component({
  selector: 'app-tabs-trigger',
  standalone: false,
  templateUrl: './tabs-trigger.component.html',
  styleUrls: ['./tabs-trigger.component.scss'],
})
export class TabsTriggerComponent {
  @Input() tabId: string = '';
  @Input() isActive: boolean = false;
  @Input() disabled: boolean = false;
  @Output() tabSelected = new EventEmitter<string>();

  @HostBinding('class') get hostClasses(): string {
    return this.getTriggerClasses();
  }

  @HostBinding('attr.role') role = 'tab';

  @HostBinding('attr.tabindex') get tabindex(): number {
    return this.isActive ? 0 : -1;
  }

  @HostBinding('attr.aria-selected') get ariaSelected(): boolean {
    return this.isActive;
  }

  onTabClick(): void {
    if (!this.disabled) {
      this.tabSelected.emit(this.tabId);
    }
  }

  private getTriggerClasses(): string {
    const baseClasses =
      'inline-flex items-center justify-center whitespace-nowrap rounded-sm px-3 py-1.5 text-sm font-medium ring-offset-background transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50';

    const stateClasses = this.isActive
      ? 'bg-background text-foreground shadow-sm'
      : 'text-muted-foreground hover:text-foreground';

    return `${baseClasses} ${stateClasses} tabs-trigger-element`;
  }
}
