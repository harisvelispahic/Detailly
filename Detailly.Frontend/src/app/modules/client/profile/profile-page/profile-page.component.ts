import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { VehiclesApiService } from '../../../../api-services/vehicles/vehicles-api.service';
import { AddressesApiService } from '../../../../api-services/addresses/addresses-api.service';
import { PaymentsService } from '../../../../api-services/payments/payments-api.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';

import { GetUserByIdQueryDto } from '../../../../api-services/users/users-api.model';
import { ListMyVehiclesQueryDto } from '../../../../api-services/vehicles/vehicles-api.model';
import { ListMyAddressesQueryDto } from '../../../../api-services/addresses/addresses-api.model';
import { WalletDto } from '../../../../api-services/payments/payments-api.service';

import { EditProfileDialogComponent } from '../edit-profile-dialog/edit-profile-dialog.component';
import { VehicleDialogComponent } from '../vehicle-dialog/vehicle-dialog.component';
import { AddressDialogComponent } from '../address-dialog/address-dialog.component';

@Component({
  selector: 'app-profile-page',
  standalone: false,
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss',
})
export class ProfilePageComponent implements OnInit {
  user?: GetUserByIdQueryDto;
  vehicles: ListMyVehiclesQueryDto[] = [];
  addresses: ListMyAddressesQueryDto[] = [];
  wallet?: WalletDto;

  isLoadingUser = false;
  isLoadingVehicles = false;
  isLoadingAddresses = false;
  isLoadingWallet = false;
  isDeletingVehicleId?: number;
  isDeletingAddressId?: number;

  constructor(
    private auth: AuthFacadeService,
    private users: UsersApiService,
    private vehiclesApi: VehiclesApiService,
    private addressesApi: AddressesApiService,
    private payments: PaymentsService,
    private dialog: MatDialog,
    private dialogHelper: DialogHelperService,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadUser();
    this.loadVehicles();
    this.loadAddresses();
    this.loadWallet();
  }

  // ---- User ----

  loadUser(): void {
    const id = this.auth.currentUser()?.userId;
    if (!id) return;
    this.isLoadingUser = true;
    this.users.getById(id).subscribe({
      next: (u) => { this.user = u; this.isLoadingUser = false; },
      error: () => { this.isLoadingUser = false; },
    });
  }

  openEditProfile(): void {
    if (!this.user) return;
    const ref = this.dialog.open(EditProfileDialogComponent, {
      data: { user: this.user, isFleet: this.auth.isFleet() },
      width: '560px',
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.loadUser();
        this.snackBar.open('Profile updated.', 'Close', { duration: 3000 });
      }
    });
  }

  // ---- Vehicles ----

  loadVehicles(): void {
    this.isLoadingVehicles = true;
    this.vehiclesApi.listMine().subscribe({
      next: (res) => { this.vehicles = res.items; this.isLoadingVehicles = false; },
      error: () => { this.isLoadingVehicles = false; },
    });
  }

  openAddVehicle(): void {
    const ref = this.dialog.open(VehicleDialogComponent, { data: {}, width: '560px' });
    ref.afterClosed().subscribe((saved) => {
      if (saved) { this.loadVehicles(); this.snackBar.open('Vehicle added.', 'Close', { duration: 3000 }); }
    });
  }

  openEditVehicle(vehicle: ListMyVehiclesQueryDto): void {
    const ref = this.dialog.open(VehicleDialogComponent, { data: { vehicle }, width: '560px' });
    ref.afterClosed().subscribe((saved) => {
      if (saved) { this.loadVehicles(); this.snackBar.open('Vehicle updated.', 'Close', { duration: 3000 }); }
    });
  }

  deleteVehicle(vehicle: ListMyVehiclesQueryDto): void {
    this.dialogHelper.confirmDelete(`${vehicle.brand} ${vehicle.model}`).subscribe((result) => {
      if (result?.button !== DialogButton.DELETE) return;
      this.isDeletingVehicleId = vehicle.id;
      this.vehiclesApi.delete(vehicle.id).subscribe({
        next: () => {
          this.vehicles = this.vehicles.filter((v) => v.id !== vehicle.id);
          this.isDeletingVehicleId = undefined;
          this.snackBar.open('Vehicle removed.', 'Close', { duration: 3000 });
        },
        error: () => {
          this.isDeletingVehicleId = undefined;
          this.snackBar.open('Failed to remove vehicle.', 'Close', { duration: 4000 });
        },
      });
    });
  }

  // ---- Addresses ----

  loadAddresses(): void {
    this.isLoadingAddresses = true;
    this.addressesApi.listMine().subscribe({
      next: (res) => { this.addresses = res.items; this.isLoadingAddresses = false; },
      error: () => { this.isLoadingAddresses = false; },
    });
  }

  openAddAddress(): void {
    const ref = this.dialog.open(AddressDialogComponent, { data: {}, width: '560px' });
    ref.afterClosed().subscribe((saved) => {
      if (saved) { this.loadAddresses(); this.snackBar.open('Address added.', 'Close', { duration: 3000 }); }
    });
  }

  openEditAddress(address: ListMyAddressesQueryDto): void {
    const ref = this.dialog.open(AddressDialogComponent, { data: { address }, width: '560px' });
    ref.afterClosed().subscribe((saved) => {
      if (saved) { this.loadAddresses(); this.snackBar.open('Address updated.', 'Close', { duration: 3000 }); }
    });
  }

  deleteAddress(address: ListMyAddressesQueryDto): void {
    this.dialogHelper.confirmDelete(address.street).subscribe((result) => {
      if (result?.button !== DialogButton.DELETE) return;
      this.isDeletingAddressId = address.id;
      this.addressesApi.delete(address.id).subscribe({
        next: () => {
          this.addresses = this.addresses.filter((a) => a.id !== address.id);
          this.isDeletingAddressId = undefined;
          this.snackBar.open('Address removed.', 'Close', { duration: 3000 });
        },
        error: () => {
          this.isDeletingAddressId = undefined;
          this.snackBar.open('Failed to remove address.', 'Close', { duration: 4000 });
        },
      });
    });
  }

  // ---- Wallet ----

  loadWallet(): void {
    this.isLoadingWallet = true;
    this.payments.getMyWallet().subscribe({
      next: (w) => { this.wallet = w; this.isLoadingWallet = false; },
      error: () => { this.isLoadingWallet = false; },
    });
  }

  // ---- Helpers ----

  get userInitials(): string {
    if (!this.user) return '?';
    return `${this.user.firstName.charAt(0)}${this.user.lastName.charAt(0)}`.toUpperCase();
  }

  get userFullName(): string {
    if (!this.user) return '';
    return `${this.user.firstName} ${this.user.lastName}`;
  }

  get accountType(): string {
    const u = this.auth.currentUser();
    if (!u) return '';
    if (u.isAdmin) return 'Administrator';
    if (u.isManager) return 'Manager';
    if (u.isEmployee) return 'Employee';
    if (u.isFleet) return 'Fleet Account';
    return 'Customer Account';
  }

  get isFleet(): boolean {
    return this.auth.isFleet();
  }

  formatPhoneDisplay(phone: string | null | undefined): string | null {
    if (!phone) return null;
    const stripped = phone.startsWith('+') ? phone.slice(1) : phone;
    const codes = [
      '387','385','386','383','382','381','380','389','359','355','420','966','971',
      '91','90','86','81','61','55','52','49','48','47','46','45','44','43','41',
      '40','39','36','34','33','32','31','30','27','7','1',
    ];
    const match = codes.find(c => stripped.startsWith(c));
    if (!match) return phone;
    const digits = stripped.slice(match.length);
    return `+${match} ${this.groupDigits(digits)}`;
  }

  private groupDigits(digits: string): string {
    if (digits.length <= 2) return digits;
    const g1   = digits.slice(0, 2);
    const g2   = digits.slice(2, 5);
    const rest = digits.slice(5);
    const parts = [g1];
    if (g2) parts.push(g2);
    if (rest) {
      if (rest.length <= 3) {
        parts.push(rest);
      } else {
        for (let i = 0; i < rest.length; i += 2) {
          parts.push(rest.slice(i, i + 2));
        }
      }
    }
    return parts.join(' ');
  }
}
