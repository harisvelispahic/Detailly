import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-tabs-list',
  standalone: false,
  templateUrl: './tabs-list.component.html',
  styleUrls: ['./tabs-list.component.scss'],
})
export class TabsListComponent {
  @HostBinding('class') get hostClasses(): string {
    return 'tabs-list-element';
  }

  @HostBinding('attr.role') role = 'tablist';
}
