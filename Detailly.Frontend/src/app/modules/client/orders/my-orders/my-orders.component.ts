import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrdersApiService } from '../../../../api-services/orders/orders-api.service';
import { GetMyOrdersDto, GetMyOrdersRequest, GetMyOrdersResponse } from '../../../../api-services/orders/orders-api.models';

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
  items: GetMyOrdersDto[] = [];

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

    const request = new GetMyOrdersRequest();
    request.paging.page = this.page;
    request.paging.pageSize = this.pageSize;

    this.ordersApi.getMy(request).subscribe({
      next: (res: GetMyOrdersResponse) => {
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
