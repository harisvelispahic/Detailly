import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrdersApiService } from '../../../../api-services/orders/orders-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  standalone: false,
})
export class OrderDetailsComponent implements OnInit {
  id!: number;

  isLoading = false;
  error?: string;

  order: any;

  cancelReason = '';
  isCancelling = false;

  constructor(
    private route: ActivatedRoute,
    private ordersApi: OrdersApiService,
    private toaster: ToasterService,
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.error = undefined;

    this.ordersApi.getDetails(this.id).subscribe({
      next: (res) => {
        this.order = res;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load order details.';
        this.isLoading = false;
      },
    });
  }

  canCancel(): boolean {
    const s = (this.order?.status ?? '').toLowerCase();
    // based on your backend rules: pending/paid/shipped allowed; delivered/cancelled not
    return s === 'pending' || s === 'paid' || s === 'shipped';
  }

  cancel(): void {
    if (!this.canCancel()) return;

    const ok = confirm('Cancel this order? A refund may apply depending on status.');
    if (!ok) return;

    this.isCancelling = true;

    this.ordersApi.cancelOrder(this.id, this.cancelReason || null).subscribe({
      next: () => {
        this.toaster.success(
          '✅ Order cancelled. If eligible, refund will be processed automatically.',
        );
        this.cancelReason = '';
        this.isCancelling = false;
        this.load();
      },
      error: (err) => {
        this.isCancelling = false;
        this.toaster.error('❌ Cancel failed. ' + (err?.error?.message ?? ''));
      },
    });
  }
}
