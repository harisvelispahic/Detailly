# Angular UI Library - Component Reference

## Scope

This document describes the actual shared UI components in `src/app/modules/shared/components/ui/`.

It also notes where the app currently depends on global theme styling and Angular Material instead of these primitives.

## Global Theme Context

Before using the components, understand that their styling depends on the global CSS variables defined in `src/styles.scss`.

The most important tokens are:

- `--background`
- `--foreground`
- `--card`
- `--primary`
- `--secondary`
- `--muted-foreground`
- `--border`
- `--ring`
- `--success`
- `--warning`
- `--info`
- `--destructive`
- `--gradient-primary`
- `--gradient-card`
- `--shadow-card`
- `--shadow-elevated`
- `--shadow-hero`

If those tokens change, the shared components change with them.

## Export Path

All UI components are exported via `SharedModule`.

Typical module setup:

```ts
import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared-module';

@NgModule({
  imports: [SharedModule],
})
export class FeatureModule {}
```

## Component Catalog

### app-button

Selector:

```html
<app-button></app-button>
```

Inputs:

- `variant: ButtonVariant = 'default'`
- `size: ButtonSize = 'default'`
- `disabled = false`
- `type: 'button' | 'submit' | 'reset' = 'button'`
- `ariaLabel?: string`

Outputs:

- `clicked: EventEmitter<MouseEvent>`

Variants:

- `default`
- `destructive`
- `outline`
- `secondary`
- `ghost`
- `link`
- `gradient`
- `hero`
- `hero-outline`
- `glass`
- `success`

Sizes:

- `default`
- `sm`
- `lg`
- `xl`
- `icon`

Behavior notes:

- Uses content projection.
- Icon placement is determined by projected markup order.
- Emits `clicked` only when `disabled === false`.
- Host gets `app-button-wrapper`; actual styling is applied to the inner `<button>`.

Visual notes:

- `default`, `gradient`, and `hero` use the purple brand direction.
- `glass` uses blurred translucent card styling.
- `outline` and `hero-outline` both use border-based treatments, but `hero-outline` is intended for CTA-style layouts.

Example:

```html
<app-button variant="hero" size="lg" (clicked)="bookNow()">
  Book Now
  <mat-icon>arrow_forward</mat-icon>
</app-button>
```

### app-badge

Selector:

```html
<app-badge></app-badge>
```

Inputs:

- `variant: BadgeVariant = 'default'`

Variants:

- `default`
- `secondary`
- `destructive`
- `outline`
- `success`
- `warning`
- `info`
- `pending`
- `confirmed`
- `completed`
- `cancelled`
- `glass`

Behavior notes:

- Text is uppercase by default because the base badge style sets `text-transform: uppercase`.
- Best suited for status chips and compact metadata.
- Host gets the computed badge classes; content is rendered inside the child `<span>`.

Example:

```html
<app-badge variant="completed">Completed</app-badge>
```

### app-card

Selector:

```html
<app-card></app-card>
```

Inputs:

- `variant: CardVariant = 'default'`

Variants:

- `default`
- `elevated`
- `interactive`
- `glass`
- `gradient`
- `outline`

Subcomponents:

- `app-card-header`
- `app-card-title`
- `app-card-description`
- `app-card-content`
- `app-card-footer`

Behavior notes:

- `app-card` is the outer variant carrier.
- Layout spacing comes from subcomponents, not the parent.
- `interactive` adds hover lift and purple glow.
- `glass` uses translucent blur styling.

Example:

```html
<app-card variant="interactive">
  <app-card-header>
    <app-card-title>Premium Package</app-card-title>
    <app-card-description>Interior and exterior detail</app-card-description>
  </app-card-header>

  <app-card-content>
    <p>$149</p>
  </app-card-content>

  <app-card-footer>
    <app-button variant="hero">Book Now</app-button>
  </app-card-footer>
</app-card>
```

### app-card-header

Purpose:

- top spacing block for a card
- vertical layout with a small gap between title/description rows

Notes:

- padding: `1.5rem`
- flex column layout

### app-card-title

Purpose:

- main heading inside a card

Notes:

- uses `Space Grotesk`
- size: `1.5rem`
- weight: `600`

### app-card-description

Purpose:

- muted descriptive text inside a card header or content area

Notes:

- size: `0.875rem`
- color: `hsl(var(--muted-foreground))`

### app-card-content

Purpose:

- main content area

Notes:

- padding: `1.5rem`
- top padding removed to sit directly below header when both are used

### app-card-footer

Purpose:

- bottom action row

Notes:

- flex row
- gap: `1rem`
- padding: `1.5rem`
- top padding removed

### app-input

Selector:

```html
<app-input></app-input>
```

Inputs:

- `type = 'text'`
- `placeholder = ''`
- `disabled = false`
- `required = false`
- `id?: string`
- `name?: string`
- `value = ''`
- `minLength?: number`
- `maxLength?: number`
- `pattern?: string`

Important limitation:

`app-input` is **not** a `ControlValueAccessor`.

That means:

- no `[(ngModel)]`
- no `[formControl]`
- no `formControlName`
- no output for value changes

Use it today for presentational/static markup or extend it before adopting it for real Angular forms.

Example:

```html
<app-label for="promo-code">Promo Code</app-label>
<app-input id="promo-code" placeholder="SPRING25"></app-input>
```

### app-label

Selector:

```html
<app-label></app-label>
```

Inputs:

- `for?: string`
- `required = false`

Behavior notes:

- renders a label with optional `*`
- suitable companion for `app-input` and `app-textarea`

Example:

```html
<app-label for="email" [required]="true">Email</app-label>
```

### app-textarea

Selector:

```html
<app-textarea></app-textarea>
```

Inputs:

- `placeholder = ''`
- `disabled = false`
- `required = false`
- `id?: string`
- `name?: string`
- `value = ''`
- `minLength?: number`
- `maxLength?: number`
- `rows = 4`

Important limitation:

Like `app-input`, this is **not** a `ControlValueAccessor`.

Use it for:

- static/demonstration markup
- uncontrolled text areas

Do not document it as a reactive-forms replacement until that work is actually done.

### app-separator

Selector:

```html
<app-separator></app-separator>
```

Inputs:

- `orientation: 'horizontal' | 'vertical' = 'horizontal'`
- `decorative = true`

Behavior notes:

- adds `role="separator"` only when `decorative` is false
- exposes `aria-orientation`
- horizontal separator is full-width 1px
- vertical separator is full-height 1px

Example:

```html
<app-separator></app-separator>
<app-separator orientation="vertical" [decorative]="false"></app-separator>
```

### app-container

Selector:

```html
<app-container></app-container>
```

Inputs:

- `size: 'sm' | 'md' | 'lg' | 'xl' | 'full' = 'lg'`
- `padding = true`

Current size mapping:

- `sm -> 24rem`
- `md -> 28rem`
- `lg -> 56rem`
- `xl -> var(--page-max-width)`
- `full -> 100%`

Behavior notes:

- the host element receives the sizing class
- the inner `.container-wrapper` just renders projected content
- `padding` toggles horizontal padding classes

Example:

```html
<app-container size="xl">
  <section>...</section>
</app-container>
```

### app-tabs

Selector:

```html
<app-tabs></app-tabs>
```

Inputs:

- `activeTab = ''`

Outputs:

- `activeTabChange: EventEmitter<string>`

Important note:

`app-tabs` is currently a thin wrapper. It does not automatically coordinate triggers and content. Consumers still manage the state manually.

### app-tabs-list

Purpose:

- tab strip container

Behavior notes:

- inline-flex layout
- muted background
- rounded shell around triggers

### app-tabs-trigger

Inputs:

- `tabId = ''`
- `isActive = false`
- `disabled = false`

Outputs:

- `tabSelected: EventEmitter<string>`

Accessibility:

- `role="tab"`
- `tabindex` is `0` for active, `-1` for inactive
- `aria-selected` reflects `isActive`

Important note:

The inner `<button>` uses `display: contents`, so the host classes carry most of the visible styling.

### app-tabs-content

Inputs:

- `tabId = ''`
- `isActive = false`

Accessibility / behavior:

- `role="tabpanel"`
- `data-state` becomes `active` or `inactive`
- `display` is toggled to `block` / `none`

## Valid Tabs Example

```ts
export class BookingPageComponent {
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
    <p>Service content</p>
  </app-tabs-content>

  <app-tabs-content tabId="addons" [isActive]="activeTab === 'addons'">
    <p>Add-on content</p>
  </app-tabs-content>
</app-tabs>
```

## Relationship to Angular Material

The app currently uses both:

- shared `app-*` primitives
- themed Angular Material components

Use Material when you need:

- `mat-form-field`
- `matInput`
- dialogs
- snackbars
- tables
- paginator
- select/menu patterns

Use the shared primitives when you want:

- token-driven composition
- lightweight reusable building blocks
- card/button/badge/container structure without Material markup

## Do Not Assume

Do not assume the following unless the implementation changes:

- Tailwind is active
- `app-input` supports reactive forms
- `app-textarea` supports reactive forms
- `app-tabs` auto-wires child state
- icon ordering is automatic

If any of those become true later, update this file first.
