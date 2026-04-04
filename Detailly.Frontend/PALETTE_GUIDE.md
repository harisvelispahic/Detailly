# Palette Guide

## Overview

This project currently uses **two palette layers**:

1. `src/styles.scss`
   This is the **runtime theme layer**.
   It defines CSS custom properties in `:root`, for example:
   - `--primary`
   - `--accent`
   - `--background`
   - `--border`

2. `src/styles/_palette-bridge.scss`
   This is the **Sass bridge layer**.
   It defines Sass variables such as:
   - `$primary-color`
   - `$card`
   - `$text-primary`
   - `$shadow-primary`

These two files are **not automatically linked**.

`styles.scss` does **not import** `_palette-bridge.scss`, and `_palette-bridge.scss` does **not read** values from `styles.scss`.

That is intentional for now.

## Temporary Warning

`src/styles/_palette-bridge.scss` should be treated as **temporary**.

It exists only to keep older Sass-based component styles working while the project still contains legacy SCSS that expects Sass variables like `$primary-color`, `$card`, and `$shadow-primary`.

It is **not** the desired end state of the theme system.

The long-term goal should be:

1. move component styles to direct CSS-variable usage
2. stop depending on `_palette-bridge.scss`
3. keep palette ownership in `src/styles.scss`

## Why There Are Two Layers

There are two different styling styles in the codebase:

1. Newer/global styles use CSS variables directly:

```scss
color: hsl(var(--primary));
border-color: hsl(var(--border));
```

2. Older admin/shared component styles were written with Sass variables and Sass functions:

```scss
background: $card;
box-shadow: $shadow-primary;
```

Sass variables are resolved at **build time**.
CSS variables are resolved at **runtime**.

Because of that, legacy component SCSS cannot cleanly consume `:root` CSS variables everywhere without being rewritten selector-by-selector.

So `_palette-bridge.scss` exists as a **compatibility layer**:
it mirrors the approved palette using Sass variables, and legacy component SCSS imports that bridge with `@use`.

## How A Component Gets Colors

Example: `src/app/modules/admin/catalogs/products/products-add/products-add.component.scss`

At the top it has:

```scss
@use '../../../../../../styles/palette-bridge' as *;
```

That makes bridge variables available inside that component:

```scss
background: $card;
box-shadow: $shadow-sm;
color: $text-primary;
```

So the flow is:

1. `_palette-bridge.scss` defines Sass color variables.
2. A component `@use`s `_palette-bridge.scss`.
3. Sass compiles those values into the component CSS.

Separately, `styles.scss` defines global CSS variables that are used by:

- global styles
- utility classes
- Material overrides
- components that directly use `hsl(var(--...))`

## Important Limitation

Right now, if you want a full palette change, you should update:

1. `src/styles.scss`
2. `src/styles/_palette-bridge.scss`

That keeps:

- runtime CSS-variable styles
- legacy Sass-variable styles

in sync.

If you only update `styles.scss`, some legacy admin/shared components will still keep the old hue family.

## Red Palette Demo

If you want to switch from the current purple palette to a red palette, change these values.

### 1. Update `src/styles.scss`

Replace the primary/accent-related values with something like:

```scss
--primary: 0 72% 51%;
--primary-foreground: 0 0% 100%;

--accent: 8 85% 60%;
--accent-foreground: 0 0% 100%;

--ring: 0 72% 51%;

--sidebar-primary: 0 72% 51%;
--sidebar-ring: 0 72% 51%;

--gradient-primary: linear-gradient(
  135deg,
  hsl(8 85% 58%) 0%,
  hsl(0 72% 51%) 50%,
  hsl(350 74% 45%) 100%
);

--gradient-accent: linear-gradient(135deg, hsl(8 90% 62%) 0%, hsl(0 72% 51%) 100%);
```

### 2. Update `src/styles/_palette-bridge.scss`

Replace the matching bridge values with:

```scss
$primary-color: hsl(0 72% 51%);
$primary-light: hsl(8 85% 60%);
$primary-dark: hsl(350 74% 45%);

$secondary-color: hsl(0 62% 56%);
$accent-color: hsl(8 85% 60%);

$hover-bg: hsl(0 72% 51% / 0.08);
$primary-soft: hsl(0 72% 51% / 0.08);
$primary-soft-strong: hsl(0 72% 51% / 0.16);
$primary-border-soft: hsl(0 72% 51% / 0.3);

$shadow-primary: 0 18px 34px -18px hsl(0 72% 51% / 0.4);

$gradient-primary: linear-gradient(
  135deg,
  hsl(8 85% 58%) 0%,
  hsl(0 72% 51%) 50%,
  hsl(350 74% 45%) 100%
);

$gradient-accent: linear-gradient(135deg, $primary-light 0%, $primary-color 100%);
```

## Which File Is The Real Source Of Truth?

Today, the practical answer is:

- `src/styles.scss` is the source of truth for the **runtime theme**
- `src/styles/_palette-bridge.scss` is the source of truth for the **legacy Sass components**

So the palette is currently **mirrored**, not fully single-sourced.

## If You Want A Cleaner Future Setup

The long-term cleanup would be:

1. Gradually remove bridge usage from component SCSS.
2. Replace Sass palette variables with direct CSS-variable usage:

```scss
background: hsl(var(--card));
color: hsl(var(--foreground));
box-shadow: 0 18px 34px -18px hsl(var(--primary) / 0.4);
```

3. Delete `_palette-bridge.scss` once no components depend on it.

At that point, `src/styles.scss` would become the only palette file.
