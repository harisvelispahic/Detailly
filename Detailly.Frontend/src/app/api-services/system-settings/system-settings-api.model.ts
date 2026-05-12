export interface SystemSettingsDto {
  standardWalletBonusPercent: number;
  fleetWalletBonusPercent: number;
  reviewWindowDays: number;
  baseFleetDiscountPercent: number;
  perVehicleFleetDiscountPercent: number;
  maxFleetDiscountPercent: number;
}

export interface UpdateSystemSettingsCommand {
  standardWalletBonusPercent: number;
  fleetWalletBonusPercent: number;
  reviewWindowDays: number;
  baseFleetDiscountPercent: number;
  perVehicleFleetDiscountPercent: number;
  maxFleetDiscountPercent: number;
}
