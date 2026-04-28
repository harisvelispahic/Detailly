import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import {
  DialogConfig,
  DialogType,
  DialogButton,
  DialogResult,
} from '../models/dialog-config.model';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class DialogHelperService {
  constructor(private dialog: MatDialog) {}

  open(config: DialogConfig): Observable<DialogResult | undefined> {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: config.width || '450px',
      disableClose: config.disableClose || false,
      data: config,
      panelClass: 'custom-dialog-container',
    });

    return dialogRef.afterClosed();
  }

  showInfo(title: string, message: string, icon?: string): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.INFO,
      title,
      message,
      icon,
      buttons: [{ type: DialogButton.OK, color: 'primary' }],
    });
  }

  showSuccess(title: string, message: string, icon?: string): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.SUCCESS,
      title,
      message,
      icon,
      buttons: [{ type: DialogButton.OK, color: 'primary' }],
    });
  }

  showError(title: string, message: string, icon?: string): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.ERROR,
      title,
      message,
      icon,
      buttons: [{ type: DialogButton.OK, color: 'warn' }],
    });
  }

  showWarning(title: string, message: string, icon?: string): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.WARNING,
      title,
      message,
      icon,
      buttons: [{ type: DialogButton.OK, color: 'primary' }],
    });
  }

  confirm(title: string, message: string, icon?: string): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.QUESTION,
      title,
      message,
      icon,
      buttons: [{ type: DialogButton.NO }, { type: DialogButton.YES, color: 'primary' }],
    });
  }

  confirmOkCancel(
    title: string,
    message: string,
    icon?: string,
  ): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.QUESTION,
      title,
      message,
      icon,
      buttons: [{ type: DialogButton.CANCEL }, { type: DialogButton.OK, color: 'primary' }],
    });
  }

  confirmDelete(itemName: string, message?: string): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.WARNING,
      title: 'Confirm Deletion',
      message: message ?? `Are you sure you want to delete "${itemName}"?`,
      icon: 'delete_forever',
      buttons: [{ type: DialogButton.CANCEL }, { type: DialogButton.DELETE, color: 'warn' }],
    });
  }

  confirmUnsavedChanges(): Observable<DialogResult | undefined> {
    return this.open({
      type: DialogType.WARNING,
      title: 'Unsaved Changes',
      message: 'You have unsaved changes. Do you want to continue?',
      icon: 'warning',
      buttons: [{ type: DialogButton.NO }, { type: DialogButton.YES, color: 'primary' }],
    });
  }

  showCustom(config: DialogConfig): Observable<DialogResult | undefined> {
    return this.open(config);
  }

  productCategory = {
    confirmDelete: (categoryName: string) => {
      return this.confirmDelete(
        categoryName,
        `Are you sure you want to delete category "${categoryName}"? This will also delete all products in this category.`,
      );
    },

    showCreateSuccess: () => this.showSuccess('Success', 'Category created successfully.'),
    showUpdateSuccess: () => this.showSuccess('Success', 'Category updated successfully.'),
    showDeleteSuccess: () => this.showSuccess('Success', 'Category deleted successfully.'),
    showCreateError: () => this.showError('Error', 'Error creating category.'),
    showUpdateError: () => this.showError('Error', 'Error updating category.'),
    showDeleteError: () => this.showError('Error', 'Error deleting category.'),
  };

  product = {
    confirmDelete: (productName: string) => {
      return this.confirmDelete(
        productName,
        `Are you sure you want to delete product "${productName}"?`,
      );
    },

    confirmCancel: () => {
      return this.confirm(
        'You have unsaved changes in the form.',
        'Do you want to cancel changes and go back?',
      );
    },

    showCreateSuccess: () => this.showSuccess('Success', 'Product created successfully.'),
    showUpdateSuccess: () => this.showSuccess('Success', 'Product updated successfully.'),
    showDeleteSuccess: () => this.showSuccess('Success', 'Product deleted successfully.'),
    showCreateError: () => this.showError('Error', 'Error creating product.'),
    showUpdateError: () => this.showError('Error', 'Error updating product.'),
    showDeleteError: () => this.showError('Error', 'Error deleting product.'),
  };
}
