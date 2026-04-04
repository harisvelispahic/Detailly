# UI Examples

This file contains examples that are valid for the current implementation.

Important rule:

- Do not copy older examples that use `[formControl]` or `[(ngModel)]` on `app-input` or `app-textarea`.
- Those wrappers do not support Angular Forms APIs yet.

## Buttons

### Primary CTA

```html
<app-button variant="hero" size="lg" (clicked)="bookNow()">
  Book Now
  <mat-icon>arrow_forward</mat-icon>
</app-button>
```

### Outline action

```html
<app-button variant="outline" (clicked)="openDetails()">
  View Details
</app-button>
```

### Icon-only button

```html
<app-button variant="ghost" size="icon" ariaLabel="Open menu" (clicked)="toggleMenu()">
  <mat-icon>menu</mat-icon>
</app-button>
```

### Status action

```html
<app-button variant="success" size="sm" (clicked)="markComplete()">
  Mark Complete
</app-button>
```

## Badges

### Order status

```html
<app-badge *ngIf="status === 'pending'" variant="pending">Pending</app-badge>
<app-badge *ngIf="status === 'confirmed'" variant="confirmed">Confirmed</app-badge>
<app-badge *ngIf="status === 'completed'" variant="completed">Completed</app-badge>
<app-badge *ngIf="status === 'cancelled'" variant="cancelled">Cancelled</app-badge>
```

### Role badge

```html
<app-badge variant="outline">Admin</app-badge>
```

### Glass badge

```html
<app-badge variant="glass">Premium</app-badge>
```

## Cards

### Basic information card

```html
<app-card variant="default">
  <app-card-header>
    <app-card-title>Profile Information</app-card-title>
    <app-card-description>Basic account details</app-card-description>
  </app-card-header>

  <app-card-content>
    <p>Name: {{ user.name }}</p>
    <p>Email: {{ user.email }}</p>
  </app-card-content>

  <app-card-footer>
    <app-button variant="outline">Edit</app-button>
  </app-card-footer>
</app-card>
```

### Interactive product/service card

```html
<app-card variant="interactive">
  <app-card-header>
    <app-card-title>Ceramic Coating</app-card-title>
    <app-card-description>Long-term gloss and protection</app-card-description>
  </app-card-header>

  <app-card-content>
    <p>$799</p>
  </app-card-content>

  <app-card-footer>
    <app-button variant="hero">
      Book Now
      <mat-icon>arrow_forward</mat-icon>
    </app-button>
  </app-card-footer>
</app-card>
```

### Glass card shell

```html
<app-card variant="glass">
  <app-card-content>
    <p>Blurred card shell for overlays or premium callouts.</p>
  </app-card-content>
</app-card>
```

## Labels, Inputs, and Textareas

### Presentational input block

Use this pattern only when you do not need Angular Forms integration yet.

```html
<div class="field">
  <app-label for="voucher">Voucher Code</app-label>
  <app-input id="voucher" placeholder="SPRING25"></app-input>
</div>
```

### Presentational textarea block

```html
<div class="field">
  <app-label for="notes">Booking Notes</app-label>
  <app-textarea id="notes" [rows]="5" placeholder="Any extra details..."></app-textarea>
</div>
```

### When to use Material instead

Use Angular Material if you need a working reactive form field today.

```html
<mat-form-field appearance="outline" class="full-width">
  <mat-label>Email</mat-label>
  <input matInput formControlName="email" type="email" />
</mat-form-field>
```

## Separator

### Horizontal divider

```html
<p>Section A</p>
<app-separator></app-separator>
<p>Section B</p>
```

### Vertical divider

```html
<div class="row">
  <span>Monthly</span>
  <app-separator orientation="vertical" [decorative]="false"></app-separator>
  <span>Yearly</span>
</div>
```

## Container

### Standard content wrapper

```html
<app-container size="xl">
  <section>
    <h1>Landing Section</h1>
    <p>Content inside the project-wide max width container.</p>
  </section>
</app-container>
```

### Tight content wrapper

```html
<app-container size="sm">
  <app-card>
    <app-card-content>Compact panel content</app-card-content>
  </app-card>
</app-container>
```

## Tabs

### Manual tabs setup

Current tabs require the consuming component to own the active tab state.

```ts
export class BookingWizardComponent {
  activeTab = 'services';
}
```

```html
<app-tabs [activeTab]="activeTab">
  <app-tabs-list>
    <app-tabs-trigger
      tabId="services"
      [isActive]="activeTab === 'services'"
      (tabSelected)="activeTab = $event"
    >
      Services
    </app-tabs-trigger>

    <app-tabs-trigger
      tabId="addons"
      [isActive]="activeTab === 'addons'"
      (tabSelected)="activeTab = $event"
    >
      Add-ons
    </app-tabs-trigger>
  </app-tabs-list>

  <app-tabs-content tabId="services" [isActive]="activeTab === 'services'">
    <app-card>
      <app-card-content>Service options go here.</app-card-content>
    </app-card>
  </app-tabs-content>

  <app-tabs-content tabId="addons" [isActive]="activeTab === 'addons'">
    <app-card>
      <app-card-content>Add-on options go here.</app-card-content>
    </app-card>
  </app-tabs-content>
</app-tabs>
```

## Page-level Material Example

The current app often uses Material directly but themed through `src/styles.scss`.

Example from the current direction:

```html
<button mat-raised-button color="primary">
  Book Now
  <mat-icon>arrow_forward</mat-icon>
</button>
```

That is valid and consistent with the current UI architecture because global Material overrides align it with the Detailly design.

## Common Mistakes to Avoid

### Do not do this with app-input

```html
<!-- Invalid for the current implementation -->
<app-input [formControl]="form.get('email')"></app-input>
```

### Do not assume app-tabs is self-wiring

```html
<!-- Incomplete -->
<app-tabs [activeTab]="activeTab">
  <app-tabs-list>...</app-tabs-list>
</app-tabs>
```

You still need:

- `(tabSelected)="activeTab = $event"` on triggers
- `[isActive]` bindings on triggers and content

### Do not assume icon direction is automatic

```html
<!-- Icon on the left -->
<app-button>
  <mat-icon>arrow_forward</mat-icon>
  Continue
</app-button>

<!-- Icon on the right -->
<app-button>
  Continue
  <mat-icon>arrow_forward</mat-icon>
</app-button>
```

## Recommended Usage Patterns Right Now

- use `app-button`, `app-badge`, `app-card`, `app-container`, and `app-separator` freely
- use `app-input` / `app-textarea` only for presentational blocks
- use Material for real form behavior and richer components already present in the app
