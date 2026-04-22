import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { VehiclesApiService } from '../../../../api-services/vehicles/vehicles-api.service';
import { AddressesApiService } from '../../../../api-services/addresses/addresses-api.service';
import { PaymentsService } from '../../../../api-services/payments/payments-api.service';

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
      next: (u) => {
        this.user = u;
        this.isLoadingUser = false;
      },
      error: () => {
        this.isLoadingUser = false;
      },
    });
  }

  openEditProfile(): void {
    if (!this.user) return;
    const ref = this.dialog.open(EditProfileDialogComponent, {
      data: this.user,
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
      next: (res) => {
        this.vehicles = res.items;
        this.isLoadingVehicles = false;
      },
      error: () => {
        this.isLoadingVehicles = false;
      },
    });
  }

  openAddVehicle(): void {
    const ref = this.dialog.open(VehicleDialogComponent, {
      data: {},
      width: '560px',
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.loadVehicles();
        this.snackBar.open('Vehicle added.', 'Close', { duration: 3000 });
      }
    });
  }

  openEditVehicle(vehicle: ListMyVehiclesQueryDto): void {
    const ref = this.dialog.open(VehicleDialogComponent, {
      data: { vehicle },
      width: '560px',
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.loadVehicles();
        this.snackBar.open('Vehicle updated.', 'Close', { duration: 3000 });
      }
    });
  }

  deleteVehicle(vehicle: ListMyVehiclesQueryDto): void {
    if (!confirm(`Remove ${vehicle.brand} ${vehicle.model}?`)) return;
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
  }

  // ---- Addresses ----

  loadAddresses(): void {
    this.isLoadingAddresses = true;
    this.addressesApi.listMine().subscribe({
      next: (res) => {
        this.addresses = res.items;
        this.isLoadingAddresses = false;
      },
      error: () => {
        this.isLoadingAddresses = false;
      },
    });
  }

  openAddAddress(): void {
    const ref = this.dialog.open(AddressDialogComponent, {
      data: {},
      width: '560px',
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.loadAddresses();
        this.snackBar.open('Address added.', 'Close', { duration: 3000 });
      }
    });
  }

  openEditAddress(address: ListMyAddressesQueryDto): void {
    const ref = this.dialog.open(AddressDialogComponent, {
      data: { address },
      width: '560px',
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.loadAddresses();
        this.snackBar.open('Address updated.', 'Close', { duration: 3000 });
      }
    });
  }

  deleteAddress(address: ListMyAddressesQueryDto): void {
    if (!confirm(`Remove address at ${address.street}?`)) return;
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
  }

  // ---- Wallet ----

  loadWallet(): void {
    this.isLoadingWallet = true;
    this.payments.getMyWallet().subscribe({
      next: (w) => {
        this.wallet = w;
        this.isLoadingWallet = false;
      },
      error: () => {
        this.isLoadingWallet = false;
      },
    });
  }

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
}
