import { Component, inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastNotificationData {
  message: string;
  type: ToastType;
}

@Component({
  selector: 'app-toast-notification',
  standalone: false,
  templateUrl: './toast-notification.component.html',
  styleUrl: './toast-notification.component.scss',
})
export class ToastNotificationComponent {
  data = inject<ToastNotificationData>(MAT_SNACK_BAR_DATA);
  snackBarRef = inject(MatSnackBarRef);
}
