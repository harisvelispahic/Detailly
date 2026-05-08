import { BaseComponent } from './base-component';

export abstract class BaseListComponent<TItem> extends BaseComponent {
  items: TItem[] = [];

  /**
   * Concrete data loading implementation is left to subclasses.
   */
  protected abstract loadData(): void;

  /**
   * Helper to call from ngOnInit of a subclass.
   */
  protected initList(): void {
    this.loadData();
  }
}
