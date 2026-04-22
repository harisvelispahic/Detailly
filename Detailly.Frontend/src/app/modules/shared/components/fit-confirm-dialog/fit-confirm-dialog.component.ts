import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DialogConfig, DialogButton, DialogButtonConfig, DialogType, DialogResult } from '../../models/dialog-config.model';

@Component({
  selector: 'app-fit-confirm-dialog',
  standalone: false,
  templateUrl: './fit-confirm-dialog.component.html',
  styleUrls: ['./fit-confirm-dialog.component.scss']
})
export class FitConfirmDialogComponent {
  DialogType = DialogType;

  private readonly buttonLabels: Record<string, string> = {
    [DialogButton.OK]: 'OK',
    [DialogButton.CANCEL]: 'Cancel',
    [DialogButton.YES]: 'Yes',
    [DialogButton.NO]: 'No',
    [DialogButton.CLOSE]: 'Close',
    [DialogButton.DELETE]: 'Delete',
    [DialogButton.SAVE]: 'Save'
  };

  constructor(
    public dialogRef: MatDialogRef<FitConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public config: DialogConfig
  ) {}

  onButtonClick(button: DialogButton, result?: any): void {
    const dialogResult: DialogResult = {
      button,
      data: result || this.config.data
    };
    this.dialogRef.close(dialogResult);
  }

  getIconClass(): string {
    switch (this.config.type) {
      case DialogType.SUCCESS: return 'icon-success';
      case DialogType.ERROR:   return 'icon-error';
      case DialogType.WARNING: return 'icon-warning';
      case DialogType.QUESTION: return 'icon-question';
      case DialogType.INFO:
      default: return 'icon-info';
    }
  }

  getDefaultIcon(): string {
    if (this.config.icon) {
      return this.config.icon;
    }

    switch (this.config.type) {
      case DialogType.SUCCESS: return 'check_circle';
      case DialogType.ERROR:   return 'error';
      case DialogType.WARNING: return 'warning';
      case DialogType.QUESTION: return 'help';
      case DialogType.INFO:
      default: return 'info';
    }
  }

  getButtonIcon(buttonType: DialogButton): string {
    switch (buttonType) {
      case DialogButton.OK:
      case DialogButton.YES:  return 'check';
      case DialogButton.NO:
      case DialogButton.CANCEL:
      case DialogButton.CLOSE: return 'close';
      case DialogButton.DELETE: return 'delete';
      case DialogButton.SAVE:   return 'save';
      default: return 'check';
    }
  }

  getButtonLabel(button: DialogButtonConfig): string {
    if (button.label) {
      return button.label;
    }

    return this.buttonLabels[button.type] ?? button.type;
  }

  getTitle(): string {
    return this.config.title ?? '';
  }

  getMessage(): string {
    return this.config.message ?? '';
  }
}
