import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BookingsService } from '../../../../api-services/bookings/bookings-api.service';
import { ListMyBookingsQueryDto } from '../../../../api-services/bookings/bookings-api.models';

@Component({
  selector: 'app-my-bookings-page',
  templateUrl: './my-bookings-page.component.html',
  standalone: false,
})
export class MyBookingsPageComponent implements OnInit {
  isLoading = false;
  error?: string;

  bookings: ListMyBookingsQueryDto[] = [];

  constructor(
    private bookingsService: BookingsService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.error = undefined;

    this.bookingsService.listMine().subscribe({
      next: (res) => {
        this.bookings = res ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load bookings.';
        this.isLoading = false;
      },
    });
  }

  openDetails(id: number): void {
    this.router.navigate(['/client/bookings', id]);
  }
}
