export interface VehicleCategoryDto {
  id: number;
  name: string;
  description?: string | null;
  basePriceMultiplier: number;
}
