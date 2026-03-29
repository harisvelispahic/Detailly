import { Component, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'app-tabs-content',
  standalone: false,
  templateUrl: './tabs-content.component.html',
  styleUrls: ['./tabs-content.component.scss'],
})
export class TabsContentComponent {
  @Input() tabId: string = '';
  @Input() isActive: boolean = false;

  @HostBinding('class') get hostClasses(): string {
    return 'mt-2 ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 tabs-content-element';
  }

  @HostBinding('attr.role') role = 'tabpanel';

  @HostBinding('attr.data-state') get dataState(): string {
    return this.isActive ? 'active' : 'inactive';
  }

  @HostBinding('style.display') get display(): string {
    return this.isActive ? 'block' : 'none';
  }
}
