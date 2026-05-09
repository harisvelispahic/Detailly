# Delete Policy

All domain entities use **soft-delete** (`IsDeleted = true`). Hard rows are never physically removed except for images (cleaned up from Cloudinary, then hard-deleted from the `Images` table). The global query filter in `DatabaseConfiguration.cs` automatically excludes `IsDeleted = true` rows from all EF Core queries.

---

## Entities that are never deleted

These entities are permanent records and deletion is always blocked:

| Entity | Reason |
|---|---|
| `Booking` | Core business record; entire system revolves around it |
| `Order` / `OrderItem` | Financial / legislative audit record |
| `PaymentTransaction` | Legislative audit record |
| `BookingVehicleAssignment` | Historical record linking vehicles to past bookings |
| `BookingEmployeeAssignment` | Historical record linking staff to past bookings |

---

## Delete cascade rules (handler-level, applied in code)

### Location → `DeleteLocationCommandHandler`

| Child | Rule |
|---|---|
| `EmployeeShift` (ShopLocationId) | Cascade soft-delete |
| `LocationOpeningHours` (ShopLocationId) | Cascade soft-delete |
| `Address` (AddressId — 1:1) | Cascade soft-delete |
| `Booking` (ShopLocationId) | **Prevent deletion** if any exist |

---

### ProductCategory → `DeleteProductCategoryCommandHandler`

| Child | Rule |
|---|---|
| `Product` (CategoryId) | **Prevent deletion** if any active products exist |

---

### Product → `DeleteProductCommandHandler`

| Child | Rule |
|---|---|
| `Inventory` (1:1) | Cascade soft-delete |
| `Image` (ProductId) | Cascade soft-delete |
| `SavedProduct` (ProductId) | Cascade soft-delete |
| `CartItem` (ProductId) | Cascade soft-delete; affected `Cart.TotalAmount` and `Cart.IsEmpty` are recalculated after deletion |
| `OrderItem` (ProductId) | Left intact — historical record |

---

### User → `DeleteUserCommandHandler`

Blocked entirely if the user has any bookings or orders.

| Child | Rule |
|---|---|
| `Vehicle` (ApplicationUserId) | Cascade soft-delete |
| `Address` (ApplicationUserId) | Cascade soft-delete |
| `Wallet` (ApplicationUserId, 1:1) | Cascade soft-delete |
| `Cart` (ApplicationUserId, 1:1) | Cascade soft-delete |
| `CartItem` (via Cart) | Cascade soft-delete (transitively via Cart) |
| `SavedProduct` (ApplicationUserId) | Cascade soft-delete |
| `UserExternalLogin` (ApplicationUserId) | Cascade soft-delete |
| `Booking` (CustomerId) | **Prevent deletion** if any exist |
| `Order` (ApplicationUserId) | **Prevent deletion** if any exist |
| `Review` | Left intact — review content outlives the user account |
| `Reaction` | Left intact |
| `RefreshToken` | Invalidated in practice via `TokenVersion++`; rows are not soft-deleted |

---

### Staff member → `DeleteStaffMemberCommandHandler`

Same user-level cascades as above (applied idempotently), plus:

| Child | Rule |
|---|---|
| `EmployeeShift` (EmployeeId) | Cascade soft-delete |
| `BookingEmployeeAssignment` | Left intact — historical record |

---

### ServicePackage → `DeleteServicePackageCommandHandler`

| Child | Rule |
|---|---|
| `Image` (ServicePackageId) | Hard-delete from Cloudinary + DB |
| `ServicePackageItemAssignment` (ServicePackageId) | Cascade soft-delete |
| `Review` (ServicePackageId) | Cascade soft-delete |
| `Reaction` (ServicePackageId) | Cascade soft-delete |
| `Booking` (ServicePackageId) | **Prevent deletion** if any exist |

---

### ServicePackageItem → `DeleteServicePackageItemCommandHandler`

| Child / Reference | Rule |
|---|---|
| `ServicePackageItemAssignment` (ServicePackageItemId) | **Prevent deletion** if any active (non-deleted) assignment exists |
| `BookingItem` (ServicePackageItemId) | **Prevent deletion** if any exist |

---

### VehicleCategory → `DeleteVehicleCategoryCommandHandler`

| Child | Rule |
|---|---|
| `Vehicle` (VehicleCategoryId) | **Prevent deletion** if any active vehicles exist |

---

### Vehicle → `DeleteVehicleCommandHandler`

| Child | Rule |
|---|---|
| `BookingVehicleAssignment` | Left intact — historical record |

---

### Address → `DeleteAddressCommandHandler`

| Reference | Rule |
|---|---|
| `Location` (AddressId) | **Prevent deletion** — delete the location to remove the address |
| `Booking` (ServiceAddressId) | **Prevent deletion** if any exist |
| `Order` (ShipToAddressId) | **Prevent deletion** if any exist |

---

## Transitivity

Cascades compose. The key chains are:

- **User → Cart → CartItems**: deleting a user soft-deletes the cart which soft-deletes its items.
- **Location → Address**: the location's owned address is soft-deleted as part of location deletion.
- **ServicePackage → Images**: images are hard-deleted (Cloudinary + DB), not soft-deleted.
- **Location blocks on Booking, User blocks on Booking/Order**: outer guard covers inner-level guards (e.g. if a user has bookings, their addresses tied to those bookings are also protected).
