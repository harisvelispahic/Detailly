import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrdersApiService } from '../../../../api-services/orders/orders-api.service';

@Component({
  selector: 'app-my-orders',
  templateUrl: './my-orders.component.html',
  standalone: false,
})
export class MyOrdersComponent implements OnInit {
  isLoading = false;
  error?: string;

  // backend returns PageResult<T>
  total = 0;
  items: any[] = [];

  // basic paging
  page = 1;
  pageSize = 10;

  constructor(
    private ordersApi: OrdersApiService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.error = undefined;

    // Your GetMyOrdersQuery expects ?Paging.PageNumber & Paging.PageSize usually.
    // buildHttpParams will flatten objects; we’ll pass in the common shape:
    const request: any = {
      paging: { pageNumber: this.page, pageSize: this.pageSize },
    };

    this.ordersApi.getMy(request).subscribe({
      next: (res: any) => {
        this.total = res.total ?? 0;
        this.items = res.items ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load orders.';
        this.isLoading = false;
      },
    });
  }

  open(id: number) {
    this.router.navigate(['/client/orders', id]);
  }

  next() {
    if (this.page * this.pageSize >= this.total) return;
    this.page++;
    this.load();
  }

  prev() {
    if (this.page <= 1) return;
    this.page--;
    this.load();
  }
}
