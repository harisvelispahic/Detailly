# Detailly

**Vehicle Detailing Booking & E-Commerce Platform**

<p align="left">
  <img src="https://img.shields.io/badge/Backend-ASP.NET%20Core%208-5C2D91">
  <img src="https://img.shields.io/badge/Frontend-Angular%2021-DD0031">
  <img src="https://img.shields.io/badge/Database-SQL%20Server-CC2927">
  <img src="https://img.shields.io/badge/Architecture-Clean%20Architecture%20%2B%20CQRS-blue">
  <img src="https://img.shields.io/badge/Status-Active%20Development-success">
</p>

---

Detailly is a full-stack platform for managing a vehicle detailing business — combining service bookings, staff scheduling, product e-commerce, and payments in a single system. It is structured as a long-term engineering project following Clean Architecture, CQRS, and real-world modeling.

---

## Architecture

The backend is organized into five projects:

```
Detailly.Domain          → Entities, enums, domain logic
Detailly.Application     → CQRS commands/queries, handlers, validators (MediatR + FluentValidation)
Detailly.Infrastructure  → EF Core, migrations, seeders, external service integrations
Detailly.API             → REST controllers, middleware, DI configuration
Detailly.Shared          → Cross-cutting constants and utilities
```

The frontend is an Angular 21 SPA organized into lazy-loaded feature modules:

```
AuthModule     → Login, register, Google OAuth callback
PublicModule   → Landing page, booking wizard, product search, public reviews
ClientModule   → Customer dashboard (bookings, orders, wallet, profile)
StaffModule    → Staff portal (shifts, locations, packages, booking assignment)
AdminModule    → Product catalog management, order admin, system settings
SharedModule   → Reusable UI components, Material setup, pipes, services
```

---

## Features

### Booking System

- Service packages with image galleries (Cloudinary), tiered pricing, and line-item composition
- In-shop and mobile service modes; mobile jobs calculate a distance surcharge via OpenRoute Service
- Booking lifecycle: `PendingPayment → Confirmed → InProgress → Completed / Cancelled`
- Time-slot availability engine with employee shift scheduling
- Fleet customer support with tiered discounts (2–8% based on vehicle count)
- Staff assignment to bookings with role-based access
- PDF export for bookings and shift reports

### Review & Rating System

- Customers can leave a written review and a reaction (emoji-style rating) per booking
- Public reviews page and staff-facing review statistics per service package

### Payments

- Stripe card payments for both bookings and product orders, with webhook verification and idempotency
- Internal wallet (top-up via card or admin credit, then pay from balance)
- Refund support for both payment methods
- Payment intent model: hold created at booking, captured at confirmation

### Product E-Commerce

- Product catalog with categories, inventory tracking, and enable/disable controls
- Shopping cart → checkout → order lifecycle (`Pending → Paid → Shipped → Delivered / Cancelled`)
- Wishlist (saved products)
- Product search page

### Staff & Location Management

- Location CRUD with per-day opening hours
- Shift scheduling with calendar view and PDF export
- Staff member management (create accounts, assign roles)
- Vehicle category administration

### User & Profile

- JWT authentication (20-minute access tokens, 14-day refresh tokens, token versioning for revocation)
- Google OAuth external login
- Profile management: personal info, saved addresses, registered vehicles, wallet balance, change password
- Role hierarchy: Admin, Manager/Staff, Client

### Monitoring & Observability

- Sentry for error tracking and performance monitoring
- Serilog with daily rolling file output and console sink
- Angular error logging interceptor that forwards frontend errors to Sentry

---

## Technology Stack

| Layer              | Technology                                 |
| ------------------ | ------------------------------------------ |
| Backend framework  | ASP.NET Core 8                             |
| ORM                | Entity Framework Core 8                    |
| Messaging          | MediatR 12                                 |
| Validation         | FluentValidation 12                        |
| Authentication     | ASP.NET Core Identity + JWT + Google OAuth |
| Payments           | Stripe (.NET SDK + stripe-js)              |
| Image storage      | Cloudinary                                 |
| Geocoding          | OpenRouteService                           |
| Logging            | Serilog                                    |
| Error tracking     | Sentry                                     |
| Database           | SQL Server                                 |
| Frontend framework | Angular 21                                 |
| UI components      | Angular Material 21                        |
| Reactive layer     | RxJS 7.8                                   |
| Language           | TypeScript 5.9                             |

---

## Domain Model

**Booking domain:** `Booking`, `ServicePackage`, `ServicePackageItem`, `ServicePackageItemAssignment`, `Location`, `LocationOpeningHours`, `BookingItem`, `BookingEmployeeAssignment`, `BookingVehicleAssignment`, `EmployeeShift`, `Review`, `Reaction`

**Sales domain:** `Order`, `OrderItem`, `Cart`, `CartItem`, `SavedProduct`

**Catalog domain:** `Product`, `ProductCategory`, `Inventory`

**Payment domain:** `Wallet`, `PaymentTransaction`, `ProcessedWebhookEvent`

**Vehicle domain:** `Vehicle`, `VehicleCategory`

**Shared:** `ApplicationUser`, `Address`, `Image`, `SystemSettings`, `RefreshToken`, `UserExternalLogin`

---

## Project Context

Developed as an engineering-focused academic project at the Faculty of Information Technologies, Mostar. The goal is a realistic, production-quality architecture rather than a prototype — with correct domain modeling, enforced layer boundaries, and extensible design decisions throughout.

---

## Author

Software Engineering Students — Faculty of Information Technologies, Mostar
