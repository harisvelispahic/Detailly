# UI Visual Guide

## Visual System Layers

```
Global theme tokens and Material overrides
    |
    +-- Shared UI primitives (`app-*`)
    |
    +-- Page-level public/auth/admin SCSS
```

The app's visual consistency comes from all three layers together, not from the shared primitives alone.

## Theme Direction

Current direction:

- dark-only
- HSL token system
- purple/violet brand accents
- glass + gradient surfaces
- Space Grotesk for headings
- Inter for body text

Core surfaces:

- page background: very dark blue-gray
- card surfaces: dark elevated panels
- borders: soft dark gray
- primary action: purple gradient or purple solid

## Primitive Visual Roles

### Button

Visual role:

- action
- CTA
- stateful feedback through hover/focus/disabled

Primary visual treatments:

- solid purple
- gradient purple
- translucent glass
- outline shell

### Badge

Visual role:

- small status marker
- role chip
- metadata label

Primary visual treatments:

- compact uppercase pill
- semantic colors for status
- optional glass variant

### Card

Visual role:

- surface container
- content grouping
- interactive panel when needed

Primary visual treatments:

- gradient-card surface
- subtle border
- shadow-card or elevated shadow
- optional hover lift and glow

### Input / Textarea

Visual role:

- dark field shell
- token-driven border/focus styling

Current limitation:

- visual wrappers only
- not yet the app's main form-control abstraction

### Tabs

Visual role:

- compact segmented switcher
- muted strip + active pill

State model:

- controlled manually by consuming component

## Component Relationships

```
app-card
  -> app-card-header
      -> app-card-title
      -> app-card-description
  -> app-card-content
  -> app-card-footer

app-tabs
  -> app-tabs-list
      -> app-tabs-trigger
  -> app-tabs-content
```

## Shared Primitive Sizing

### Button sizing

- `sm`: short compact control
- `default`: standard action size
- `lg`: larger CTA
- `xl`: prominent CTA
- `icon`: square icon-only action

### Container sizing

- `sm`: narrow utility content
- `md`: compact forms/panels
- `lg`: standard body width
- `xl`: app-level wide layout width
- `full`: no width cap

## Color Intent Map

```
primary      -> main CTA / active emphasis
secondary    -> hover shells / muted surfaces
accent       -> brighter purple highlights
success      -> completed / verified
warning      -> pending / caution
info         -> confirmed / informational
destructive  -> cancelled / delete / error
```

## Current Public/Auth Visual Alignment

The current public/auth UI is aligned by:

- using the same tokens from `src/styles.scss`
- styling Angular Material globally
- using shared visual language in page SCSS

Examples:

- navbar glass shell
- hero gradients and purple CTAs
- auth dark card and logo treatment
- landing cards using matching border/shadow behavior

## What This Means for Future UI Work

If a new component should look like the existing app:

1. use the global tokens first
2. prefer the same border radius and shadow families
3. keep icon placement explicit in markup
4. reuse button/card/badge/container patterns where behavior is simple
5. keep Angular Material if the screen already depends on its behavior, but make it visually conform through token-aligned styling

## Visual Consistency Rules

- headings should inherit the display direction already established in global styles
- surfaces should stay dark
- buttons should not drift back to Material defaults
- borders should remain subtle, never bright gray
- interactive hover states should use lift, contrast, or purple emphasis
- icon ordering should be intentional and visible in the template

## Anti-Patterns

Avoid documenting or building new UI as if:

- the app were light-themed
- Tailwind utilities were the styling source of truth
- all forms were powered by the shared input wrappers
- Material defaults were acceptable without overrides

## Source Files to Check First

When visual behavior looks wrong, check these first:

- `src/styles.scss`
- `src/app/modules/shared/components/ui/button/button.component.scss`
- `src/app/modules/shared/components/ui/card/card.component.scss`
- `src/app/modules/shared/components/ui/badge/badge.component.scss`
- the page-level SCSS for the affected screen
