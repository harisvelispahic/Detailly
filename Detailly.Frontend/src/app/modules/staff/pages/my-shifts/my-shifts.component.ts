import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BaseListComponent } from '../../../../core/components/base-classes/base-list-component';
import { EmployeeShiftsApiService } from '../../../../api-services/employee-shifts/employee-shifts-api.service';
import { EmployeeWorkMode, MyShiftDto } from '../../../../api-services/employee-shifts/employee-shifts-api.models';

@Component({
  selector: 'app-my-shifts',
  standalone: false,
  templateUrl: './my-shifts.component.html',
  styleUrl: './my-shifts.component.scss',
})
export class MyShiftsComponent extends BaseListComponent<MyShiftDto> implements OnInit, OnDestroy {
  private shiftsApi = inject(EmployeeShiftsApiService);
  private destroy$ = new Subject<void>();

  displayedColumns = ['date', 'location', 'workMode', 'start', 'end', 'duration'];
  EmployeeWorkMode = EmployeeWorkMode;

  days = 7;
  readonly daysOptions = [3, 7, 14, 30];

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected loadData(): void {
    this.startLoading();
    this.shiftsApi.listMine(this.days).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.items = result;
        this.stopLoading();
      },
      error: () => this.stopLoading('Failed to load your shifts'),
    });
  }

  onDaysChange(days: number): void {
    this.days = days;
    this.loadData();
  }

  isToday(startUtc: string): boolean {
    const d = new Date(startUtc);
    const now = new Date();
    return (
      d.getFullYear() === now.getFullYear() &&
      d.getMonth() === now.getMonth() &&
      d.getDate() === now.getDate()
    );
  }

  isPast(startUtc: string): boolean {
    const d = new Date(startUtc);
    const now = new Date();
    const todayStart = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    return d < todayStart;
  }

  getWorkModeLabel(mode: EmployeeWorkMode): string {
    return mode === EmployeeWorkMode.InShop ? 'In Shop' : 'Mobile';
  }

  getWorkModeIcon(mode: EmployeeWorkMode): string {
    return mode === EmployeeWorkMode.InShop ? 'store' : 'directions_car';
  }

  getDuration(shift: MyShiftDto): string {
    const start = new Date(shift.startUtc);
    const end = new Date(shift.endUtc);
    const totalMinutes = Math.round((end.getTime() - start.getTime()) / 60000);
    const hours = Math.floor(totalMinutes / 60);
    const mins = totalMinutes % 60;
    return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
  }
}
