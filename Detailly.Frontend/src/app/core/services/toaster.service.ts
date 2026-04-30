import { Injectable, inject } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import {
  ToastNotificationComponent,
  ToastNotificationData,
  ToastType,
} from '../../modules/shared/components/toast-notification/toast-notification.component';

@Injectable({
  providedIn: 'root',
})
export class ToasterService {
  private snackBar = inject(MatSnackBar);

  private readonly defaultDuration = 4000;

  private readonly baseConfig: MatSnackBarConfig = {
    horizontalPosition: 'end',
    verticalPosition: 'top',
    panelClass: ['detaily-snack'],
  };

  private open(message: string, type: ToastType, duration?: number): void {
    const data: ToastNotificationData = { message, type };
    this.snackBar.openFromComponent(ToastNotificationComponent, {
      ...this.baseConfig,
      duration: duration ?? this.defaultDuration,
      data,
    });
  }

  success(message: string, duration?: number): void {
    this.open(message, 'success', duration);
  }

  error(message: string, duration?: number): void {
    this.open(message, 'error', duration);
  }

  warning(message: string, duration?: number): void {
    this.open(message, 'warning', duration);
  }

  info(message: string, duration?: number): void {
    this.open(message, 'info', duration);
  }
}
