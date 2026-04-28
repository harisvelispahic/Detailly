import { Component, Input } from '@angular/core';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';

@Component({
  selector: 'app-paginator-bar',
  standalone: false,
  templateUrl: './paginator-bar.component.html',
  styleUrl: './paginator-bar.component.scss',
})
export class PaginatorBarComponent {
  // ViewModel is any component that inherits BaseListPagedComponent
  @Input({ required: true }) vm!: BaseListPagedComponent<any, any>;
}
