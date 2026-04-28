import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import {
  ListStaffMembersQueryDto,
  ListStaffMembersRequest,
} from '../../../../api-services/staff-members/staff-members-api.models';
import { StaffMembersApiService } from '../../../../api-services/staff-members/staff-members-api.service';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import {
  StaffMemberUpsertDialogComponent,
  StaffMemberUpsertDialogData,
} from './staff-member-upsert-dialog/staff-member-upsert-dialog.component';

@Component({
  selector: 'app-staff-members',
  standalone: false,
  templateUrl: './staff-members.component.html',
  styleUrl: './staff-members.component.scss',
})
export class StaffMembersComponent
  extends BaseListPagedComponent<ListStaffMembersQueryDto, ListStaffMembersRequest>
  implements OnInit, OnDestroy
{
  private staffApi = inject(StaffMembersApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  readonly auth = inject(AuthFacadeService);
  private destroy$ = new Subject<void>();

  displayedColumns = ['name', 'username', 'email', 'phone', 'role', 'actions'];
  searchCtrl = new FormControl('');
  roleFilter: boolean | null = null;

  constructor() {
    super();
    this.request = new ListStaffMembersRequest();
  }

  ngOnInit(): void {
    this.searchCtrl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$),
    ).subscribe((val) => {
      this.request.search = val?.trim() || null;
      this.request.paging.page = 1;
      this.loadPagedData();
    });

    this.loadPagedData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected loadPagedData(): void {
    this.startLoading();
    this.staffApi.list(this.request).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.handlePageResult(res);
        this.stopLoading();
      },
      error: () => this.stopLoading('Failed to load staff members.'),
    });
  }

  onRoleFilterChange(value: boolean | null): void {
    this.roleFilter = value;
    this.request.isManager = value;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  canEdit(member: ListStaffMembersQueryDto): boolean {
    if (this.auth.isAdmin()) return true;
    return this.auth.isManager() && !member.isManager;
  }

  canDelete(member: ListStaffMembersQueryDto): boolean {
    return this.canEdit(member);
  }

  openCreateDialog(): void {
    this.dialog
      .open(StaffMemberUpsertDialogComponent, {
        width: '560px',
        maxWidth: '95vw',
        data: { member: null, isAdmin: this.auth.isAdmin() } as StaffMemberUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Staff member created.');
          this.loadPagedData();
        }
      });
  }

  openEditDialog(member: ListStaffMembersQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    this.dialog
      .open(StaffMemberUpsertDialogComponent, {
        width: '560px',
        maxWidth: '95vw',
        data: { member, isAdmin: this.auth.isAdmin() } as StaffMemberUpsertDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.toaster.success('Staff member updated.');
          this.loadPagedData();
        }
      });
  }

  deleteMember(member: ListStaffMembersQueryDto, event: MouseEvent): void {
    event.stopPropagation();
    const label = `${member.firstName} ${member.lastName}`;
    this.dialogHelper.confirmDelete(label).subscribe((result) => {
      if (result?.button !== DialogButton.DELETE) return;
      this.staffApi.delete(member.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.toaster.success('Staff member deleted.');
          this.loadPagedData();
        },
        error: () => this.toaster.error('Failed to delete staff member.'),
      });
    });
  }
}
