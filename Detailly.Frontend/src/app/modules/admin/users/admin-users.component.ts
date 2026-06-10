import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { ListUsersQueryDto, ListUsersRequest } from '../../../api-services/users/users-api.model';
import { UsersApiService } from '../../../api-services/users/users-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-admin-users',
  standalone: false,
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss',
})
export class AdminUsersComponent
  extends BaseListPagedComponent<ListUsersQueryDto, ListUsersRequest>
  implements OnInit, OnDestroy
{
  private usersApi = inject(UsersApiService);
  private toaster = inject(ToasterService);
  private destroy$ = new Subject<void>();

  displayedColumns: string[] = ['name', 'username', 'email', 'company', 'fleetStatus', 'actions'];
  searchControl = new FormControl('');

  constructor() {
    super();
    this.request = new ListUsersRequest();
  }

  ngOnInit(): void {
    this.initList();
    this.searchControl.valueChanges
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((term) => {
        if (!term || term.length >= 3) {
          this.request.search = term || null;
          this.request.paging.page = 1;
          this.loadPagedData();
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected loadPagedData(): void {
    this.startLoading();
    this.usersApi.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: () => this.stopLoading('Failed to load users'),
    });
  }

  toggleFleetStatus(user: ListUsersQueryDto): void {
    const newStatus = !user.isFleet;
    this.usersApi.setFleetStatus(user.id, { isFleet: newStatus }).subscribe({
      next: () => {
        user.isFleet = newStatus;
        this.toaster.success(
          newStatus ? `${user.firstName} ${user.lastName} granted fleet status.`
                    : `Fleet status removed from ${user.firstName} ${user.lastName}.`
        );
      },
      error: () => this.toaster.error('Failed to update fleet status.'),
    });
  }
}
