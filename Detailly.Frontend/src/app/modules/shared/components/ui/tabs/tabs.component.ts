import { Component, Input, Output, EventEmitter, HostBinding } from '@angular/core';

@Component({
  selector: 'app-tabs',
  standalone: false,
  templateUrl: './tabs.component.html',
  styleUrls: ['./tabs.component.scss'],
})
export class TabsComponent {
  @Input() activeTab: string = '';
  @Output() activeTabChange = new EventEmitter<string>();

  @HostBinding('class') get hostClasses(): string {
    return 'tabs-wrapper';
  }

  onTabChange(tabId: string): void {
    this.activeTab = tabId;
    this.activeTabChange.emit(tabId);
  }
}
