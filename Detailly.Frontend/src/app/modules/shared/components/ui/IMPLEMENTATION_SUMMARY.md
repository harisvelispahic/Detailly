# Angular UI Component Library - Implementation Summary

## Current Status

This folder documents the UI foundation that currently exists in the Angular app.

The implementation is split into two layers:

1. Shared UI primitives in `src/app/modules/shared/components/ui/`
2. Global theme and Angular Material overrides in `src/styles.scss`

That split matters. The app does not rely only on `app-*` primitives. The public landing page, auth screens, dialogs, and a lot of admin UI still use Angular Material directly, but they inherit the Detailly dark theme through global overrides.

## What Actually Exists

The shared UI layer exports 17 Angular components through `SharedModule`:

### Primitive / single-purpose components

- `app-button`
- `app-badge`
- `app-input`
- `app-label`
- `app-textarea`
- `app-separator`
- `app-container`

### Card system

- `app-card`
- `app-card-header`
- `app-card-title`
- `app-card-description`
- `app-card-content`
- `app-card-footer`

### Tabs system

- `app-tabs`
- `app-tabs-list`
- `app-tabs-trigger`
- `app-tabs-content`

## SharedModule Role

`SharedModule` is the delivery mechanism for the UI library.

It currently exports:

- all 17 UI components
- Angular Material modules from `material-modules.ts`
- `CommonModule`
- `FormsModule`
- `ReactiveFormsModule`
- `RouterModule`
- `TranslatePipe`

This means feature modules usually get both:

- the custom `app-*` primitives
- themed Angular Material building blocks

## Theme Source of Truth

The design system lives primarily in `src/styles.scss`.

Important facts:

- The app theme is dark-only.
- Core colors are defined as HSL CSS variables.
- Gradient, shadow, ring, layout, and status tokens are centralized there.
- Angular Material surfaces, buttons, inputs, dialogs, menus, selects, cards, and icon buttons are globally overridden there to match the Detailly look.
- Public/auth styling also builds on those same variables.

Key token groups currently in use:

- background / foreground
- card / popover
- primary / accent / secondary
- muted / border / input / ring
- success / warning / info / destructive
- gradient-primary / gradient-card / gradient-hero
- shadow-card / shadow-elevated / shadow-hero
- page-max-width / page-padding-x / nav heights

## What Changed Recently

These docs were updated because the implementation changed in meaningful ways:

- The shared primitives were restyled to better match the React Detailly design.
- Button, badge, input, card, and container styles were tightened around the new dark theme.
- Angular Material now has stronger global visual overrides so MDC defaults do not leak into the UI.
- The public navbar and auth/login branding now use the Detailly logo assets.
- The landing page hero, features, services, pricing, testimonials, and final CTA were adjusted toward the React references.

## Important Implementation Truths

These are the main points earlier docs got wrong.

### 1. This library is not Tailwind-driven

The current implementation uses:

- Angular components
- SCSS
- CSS variables
- Angular Material for some higher-level UI

Some examples in the old docs used Tailwind utility classes as if Tailwind were the source of truth. It is not.

### 2. `app-input` and `app-textarea` are presentational wrappers, not Angular form controls

They accept static inputs like:

- `value`
- `placeholder`
- `required`
- `minLength`
- `maxLength`

They do **not** implement:

- `ControlValueAccessor`
- `ngModel`
- `formControl`
- `valueChange`

So today they are useful for:

- static UI composition
- demos
- lightweight uncontrolled markup

They are not yet a full replacement for `matInput` in reactive forms.

### 3. Tabs are manually coordinated

`app-tabs` is currently a thin wrapper.

It exposes:

- `activeTab`
- `activeTabChange`

But it does not automatically wire child triggers/content together via injection or a shared controller. The host component still owns the state and explicitly passes `isActive` into triggers and content.

### 4. Icon order in buttons is determined by template content order

`app-button` uses content projection. If you want text followed by icon, you must write the content in that order.

Example:

```html
<app-button variant="hero">
  Book Now
  <mat-icon>arrow_forward</mat-icon>
</app-button>
```

### 5. The app uses both custom primitives and Angular Material

That is the current architecture. It is not a failure state, but it should be documented honestly.

Use the shared primitives when:

- you want simple reusable composition
- you want token-driven styling without Material markup

Use Angular Material when:

- the existing screen already uses Material patterns
- you need dialogs, snackbars, advanced form fields, menus, tables, selects, icons, or paginator behavior already present in the app

## Component-by-Component Summary

### Button

- 11 variants
- 5 sizes
- click output: `clicked`
- projected content
- icon placement controlled by markup order

### Badge

- 12 variants
- uppercase styling by default
- good fit for status chips and role chips

### Card system

- 6 variants on `app-card`
- subcomponents provide spacing and typography structure
- useful for content sections, settings blocks, and empty states

### Input / Textarea

- consistent dark theme shell
- not connected to Angular forms APIs

### Label

- simple wrapper with required asterisk support

### Separator

- horizontal or vertical
- supports decorative / semantic role mode

### Container

- size-based max width wrapper
- `xl` now follows the app-level `--page-max-width` token

### Tabs system

- visually functional
- state still manually orchestrated in the consuming component

## Known Gaps / Technical Debt

These are worth keeping in mind for future work.

- `app-input` and `app-textarea` need `ControlValueAccessor` if they are meant to replace Material/native form fields.
- The tabs system could be upgraded so `app-tabs` actually coordinates children.
- Several component classes still contain private helper methods that are no longer used. They are harmless, but they are not the runtime source of truth.
- Documentation and implementation had drifted badly before this update.
- The application still contains pages that are not visually migrated yet, even though the theme foundation is much better now.

## Recommended Usage Today

### Safe to use now

- `app-button`
- `app-badge`
- `app-card` and its subcomponents
- `app-label`
- `app-separator`
- `app-container`
- tabs, if you are fine managing state manually

### Use with caution

- `app-input`
- `app-textarea`

Use them for presentational/static markup only unless you extend them.

### Still rely on Angular Material for

- reactive forms already built around `mat-form-field` and `matInput`
- dialogs
- snackbars
- menu/select/table/paginator flows
- icon buttons and advanced admin UI

## Documentation Map

Use the docs in this order:

1. `IMPLEMENTATION_SUMMARY.md`
   - high-level truth about what exists
2. `UI_LIBRARY.md`
   - API-level reference for selectors, inputs, outputs, and styling behavior
3. `ARCHITECTURE.md`
   - how the library and global theme fit together
4. `EXAMPLES.md`
   - valid usage patterns that match the current implementation
5. `QUICK_REFERENCE.md`
   - short checklist and component lookup
6. `VISUAL_GUIDE.md`
   - hierarchy and visual relationships

## Bottom Line

The current Angular UI foundation is usable and much closer to the Detailly React look than the previous docs implied, but it is not a pure component-library architecture. The real source of truth is:

- shared `app-*` primitives
- global `styles.scss` theme tokens and Material overrides
- page-level SCSS for public/auth views

These docs now reflect that reality and should be used as the reference point going forward.
