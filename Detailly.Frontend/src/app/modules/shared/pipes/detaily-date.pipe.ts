import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'detailyDate', standalone: false })
export class DetailyDatePipe implements PipeTransform {
  private static readonly MONTHS = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December',
  ];

  transform(value: string | Date | null | undefined, format: 'short' | 'long' = 'short'): string {
    if (!value) return '';
    const date = typeof value === 'string' ? new Date(value) : value;
    const day = String(date.getDate()).padStart(2, '0');
    const month = date.getMonth();
    const year = date.getFullYear();

    if (format === 'short') {
      return `${day}/${String(month + 1).padStart(2, '0')}/${year}`;
    }
    return `${day} ${DetailyDatePipe.MONTHS[month]} ${year}`;
  }
}
