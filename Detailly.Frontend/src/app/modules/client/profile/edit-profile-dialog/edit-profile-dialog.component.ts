import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { GetUserByIdQueryDto } from '../../../../api-services/users/users-api.model';

@Component({
  selector: 'app-edit-profile-dialog',
  standalone: false,
  templateUrl: './edit-profile-dialog.component.html',
  styleUrl: './edit-profile-dialog.component.scss',
})
export class EditProfileDialogComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  error?: string;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditProfileDialogComponent>,
    private users: UsersApiService,
    @Inject(MAT_DIALOG_DATA) public data: GetUserByIdQueryDto,
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      firstName: [this.data.firstName, Validators.required],
      lastName: [this.data.lastName, Validators.required],
      username: [this.data.username, Validators.required],
      email: [this.data.email, [Validators.required, Validators.email]],
      companyName: [this.data.companyName ?? ''],
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.error = undefined;
    this.isLoading = true;

    this.users.update(this.data.id, this.form.value).subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        this.error = err?.error?.message ?? 'Update failed.';
        this.isLoading = false;
      },
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
