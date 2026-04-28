import { Component, inject } from '@angular/core';
import { LoadingBarService } from '../../../../core/services/loading-bar.service';

@Component({
  selector: 'app-loading-bar',
  standalone: false,
  templateUrl: './loading-bar.component.html',
  styleUrl: './loading-bar.component.scss',
})
export class LoadingBarComponent {
  /**
   * Inject LoadingBarService to access loading state
   * Component subscribes to loading$ observable to show/hide bar
   */
  protected loadingBar = inject(LoadingBarService);
}
