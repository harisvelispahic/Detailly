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
    const baseClass = 'tabs-trigger-element';
    const stateClass = this.isActive ? 'tabs-trigger-active' : 'tabs-trigger-inactive';
    const disabledClass = this.disabled ? 'tabs-trigger-disabled' : '';
    return `${baseClass} ${stateClass} ${disabledClass}`.trim();
  }
}
