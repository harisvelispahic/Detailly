# UI Quick Reference

## Selectors

| Selector | Purpose | Notes |
| --- | --- | --- |
| `app-button` | Action button | 11 variants, 5 sizes |
| `app-badge` | Status/label chip | Uppercase by default |
| `app-card` | Card shell | 6 variants |
| `app-card-header` | Card header block | Adds spacing/gap |
| `app-card-title` | Card heading | Space Grotesk, 1.5rem |
| `app-card-description` | Card helper text | Muted text |
| `app-card-content` | Card body | Padding with no top gap |
| `app-card-footer` | Card actions row | Flex row with gap |
| `app-input` | Presentational input | Not a form control |
| `app-label` | Label | Optional required marker |
| `app-textarea` | Presentational textarea | Not a form control |
| `app-separator` | Divider | Horizontal or vertical |
| `app-container` | Width wrapper | `xl` follows app max width |
| `app-tabs` | Tabs wrapper | Thin shell only |
| `app-tabs-list` | Tab strip | Muted rounded shell |
| `app-tabs-trigger` | Tab trigger | Manual state wiring |
| `app-tabs-content` | Tab panel | `display` toggled by `isActive` |

## Button Variants

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

## Button Sizes

- `default`
- `sm`
- `lg`
- `xl`
- `icon`

## Badge Variants

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

## Card Variants

- `default`
- `elevated`
- `interactive`
- `glass`
- `gradient`
- `outline`

## Container Sizes

- `sm -> 24rem`
- `md -> 28rem`
- `lg -> 56rem`
- `xl -> var(--page-max-width)`
- `full -> 100%`

## Component Reality Checks

### app-input

- supports static attributes only
- does not implement `ControlValueAccessor`
- do not use `[formControl]`, `formControlName`, or `[(ngModel)]`

### app-textarea

- same limitation as `app-input`

### app-tabs

- does not automatically coordinate children
- consumer must wire:
  - `activeTab`
  - `(tabSelected)`
  - `[isActive]`

### app-button

- projected markup controls icon position
- if you want icon on the right, write text first, then the icon

## Theme Source of Truth

Primary visual source:

- `src/styles.scss`

Look there for:

- color tokens
- gradients
- shadows
- layout width/padding tokens
- Angular Material theme overrides

## Use This / Avoid This

### Use this

- `app-button` for reusable action styling
- `app-badge` for statuses
- `app-card` system for structured blocks
- `app-container` for width constraints
- Angular Material for advanced working forms/dialogs/menus/tables already in the app

### Avoid this

- documenting the system as Tailwind-based
- claiming `app-input` is ready for reactive forms
- claiming tabs are automatic
- assuming the shared library is the only UI layer in the project

## New UI Component Checklist

- create component folder under `ui/`
- add `.ts`, `.html`, `.scss`
- use `@HostBinding` if host classes matter
- keep styling token-driven
- add to `SharedModule` declarations and exports
- update `UI_LIBRARY.md`
- update `EXAMPLES.md`
- update `ARCHITECTURE.md` if the component changes system boundaries

## Doc Update Checklist

When the implementation changes, re-check:

- selector names
- input/output names
- actual variant lists
- whether the component supports Angular forms
- whether examples still compile conceptually
- whether global `styles.scss` changed the visual contract

## Current Date of This Reference

Updated for the current implementation state on 2026-04-05.
