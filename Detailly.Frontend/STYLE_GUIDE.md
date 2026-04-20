# Detailly Frontend — Style Reference Guide

This is the **authoritative style reference** for implementing new features.
Before writing any component styles, read this document first.

> **Single source of truth:** [`src/styles.scss`](src/styles.scss) is the only style entry point.
> All CSS custom properties live there. Keep this guide in sync whenever you add or rename tokens.

---

## Quick-Start Rule

**Use CSS variables (`hsl(var(--...))`) in all new component SCSS.**
No imports are needed — variables are globally available.

---

## 1. Source Files

| File | Role | Lines |
|------|------|-------|
| [src/styles.scss](src/styles.scss) | Global stylesheet, CSS custom properties, Material overrides, utility classes | 916 |

`angular.json` registers only `src/styles.scss` in the build `styles` array.
Everything else flows from that single entry point.

> `src/styles/_palette-bridge.scss` and `PALETTE_GUIDE.md` have been deleted — all 13 legacy components were migrated to CSS variables.

---

## 2. CSS Custom Properties

Defined in the `:root` block of [src/styles.scss](src/styles.scss).
Use them as: `hsl(var(--token))` for colors, `var(--token)` for everything else.

### 2.1 Base Colors

| Token | Value | Use For |
|-------|-------|---------|
| `--background` | `240 15% 9%` | Page background |
| `--foreground` | `0 0% 98%` | Primary text |
| `--card` | `240 12% 12%` | Card / panel surfaces |
| `--card-foreground` | `0 0% 98%` | Text on cards |
| `--popover` | `240 12% 10%` | Popover / dropdown backgrounds |
| `--popover-foreground` | `0 0% 98%` | Text in popovers |
| `--muted` | `240 10% 20%` | Muted surface / dividers |
| `--muted-foreground` | `240 5% 60%` | Secondary / hint text |
| `--border` | `240 10% 20%` | Borders and dividers |
| `--input` | `240 10% 18%` | Form input backgrounds |
| `--ring` | `270 70% 60%` | Focus rings |

### 2.2 Brand Colors

| Token | Value | Use For |
|-------|-------|---------|
| `--primary` | `270 70% 60%` | Primary actions, highlights, active states |
| `--primary-foreground` | `0 0% 100%` | Text/icons on primary backgrounds |
| `--secondary` | `240 10% 18%` | Secondary buttons, subtle surfaces |
| `--secondary-foreground` | `0 0% 98%` | Text on secondary |
| `--accent` | `280 80% 65%` | Accent highlights, decorative elements |
| `--accent-foreground` | `0 0% 100%` | Text on accent |
| `--neutral` | `240 5% 60%` | Neutral status, disabled states |
| `--neutral-foreground` | `0 0% 100%` | Text on neutral |

### 2.3 Status / Semantic Colors

| Token | Value | Use For |
|-------|-------|---------|
| `--success` | `142 76% 36%` | Confirmed, paid, completed states |
| `--success-foreground` | `0 0% 100%` | Text on success |
| `--warning` | `38 92% 50%` | Pending, caution states |
| `--warning-foreground` | `0 0% 0%` | Text on warning (black — high contrast) |
| `--info` | `200 90% 50%` | Informational states |
| `--info-foreground` | `0 0% 100%` | Text on info |
| `--destructive` | `0 72% 51%` | Errors, delete actions, danger states |
| `--destructive-foreground` | `0 0% 100%` | Text on destructive |

### 2.4 Depth Surfaces

Use these for layering elements from background → foreground depth.

| Token | Approx Value | Depth |
|-------|-------------|-------|
| `--background` | `240 15% 9%` | Page base |
| `--surface-1` | `240 12% 14%` | First layer above page |
| `--surface-2` | `240 11% 16%` | Second layer |
| `--surface-3` | `240 10% 18%` | Third layer (same as `--input`) |
| `--surface-4` | `240 10% 20%` | Fourth layer (same as `--muted`) |

### 2.5 Sidebar Tokens

For sidebar-specific components — mirrors the main palette but scoped.

| Token | Value |
|-------|-------|
| `--sidebar-background` | `240 12% 10%` |
| `--sidebar-foreground` | `0 0% 98%` |
| `--sidebar-primary` | `270 70% 60%` |
| `--sidebar-primary-foreground` | `0 0% 100%` |
| `--sidebar-accent` | `240 10% 18%` |
| `--sidebar-accent-foreground` | `0 0% 98%` |
| `--sidebar-border` | `240 10% 20%` |
| `--sidebar-ring` | `270 70% 60%` |

### 2.6 Gradients

| Token | Use For |
|-------|---------|
| `--gradient-primary` | Primary CTA buttons, hero highlights |
| `--gradient-accent` | Accent decorations, text highlights |
| `--gradient-card` | Card backgrounds with depth |
| `--gradient-hero` | Full-page hero section background |
| `--gradient-page` | Full-page scroll background |
| `--gradient-surface` | Surface panels with subtle depth |
| `--gradient-danger-soft` | Soft error/danger backgrounds |
| `--gradient-warning-soft` | Soft warning backgrounds |

Usage: `background: var(--gradient-primary);`

### 2.7 Shadows

| Token | Use For |
|-------|---------|
| `--shadow-card` | Cards, panels |
| `--shadow-elevated` | Modals, drawers, popovers |
| `--shadow-soft` | Subtle lift effects |
| `--shadow-strong` | Heavy elevation |
| `--shadow-hero` | Hero CTA elements |
| `--shadow-primary` | Primary-colored glow shadow |
| `--shadow-warning` | Warning-tinted glow shadow |
| `--shadow-danger` | Danger-tinted glow shadow |
| `--glow-primary` | Bloom glow for primary elements |
| `--glow-accent` | Bloom glow for accent elements |

Usage: `box-shadow: var(--shadow-card);`

### 2.8 Overlay Tokens

| Token | Use For |
|-------|---------|
| `--overlay-foreground-soft` | `hsl(0 0% 100% / 0.08)` — hover highlights |
| `--overlay-foreground-medium` | `hsl(0 0% 100% / 0.12)` — active highlights |
| `--overlay-foreground-strong` | `hsl(0 0% 100% / 0.18)` — strong highlights |

### 2.9 Layout Tokens

| Token | Value | Use For |
|-------|-------|---------|
| `--radius` | `0.75rem` | Default border radius for cards/inputs |
| `--page-max-width` | `1400px` | Max content width |
| `--page-padding-x` | `1rem` (mobile), `2rem` (768px+) | Horizontal page padding |
| `--nav-height-mobile` | `4rem` | Mobile nav bar height |
| `--nav-height-desktop` | `5rem` | Desktop nav bar height |

---

## 3. How to Write New Component SCSS

### 3.1 The Standard Pattern

No imports needed. CSS variables are globally available.

```scss
.my-component {
  background: hsl(var(--card));
  border: 1px solid hsl(var(--border));
  border-radius: var(--radius);
  color: hsl(var(--foreground));
  box-shadow: var(--shadow-card);
}

.my-component__title {
  color: hsl(var(--foreground));
  font-family: 'Space Grotesk', sans-serif;
}

.my-component__subtitle {
  color: hsl(var(--muted-foreground));
  font-family: 'Inter', sans-serif;
}

.my-component__action {
  background: hsl(var(--primary));
  color: hsl(var(--primary-foreground));
}

.my-component__action:hover {
  background: var(--gradient-primary);
  box-shadow: var(--shadow-primary);
}
```

### 3.2 Opacity with CSS Variables

When you need a transparent variant of a token:

```scss
// Semi-transparent surface
background: hsl(var(--primary) / 0.08);    // soft fill
border: 1px solid hsl(var(--primary) / 0.3); // soft border
color: hsl(var(--primary));
```

This pattern covers all the "soft" variants (equivalent to Sass bridge's `$primary-soft`, `$primary-border-soft`, etc.)

### 3.3 Status Badge Pattern

Reference: [src/app/modules/admin/orders/admin-orders-status-styles.scss](src/app/modules/admin/orders/admin-orders-status-styles.scss)

```scss
.status-badge {
  display: inline-flex;
  align-items: center;
  padding: 4px 12px;
  border-radius: 100px;
  font-size: 0.75rem;
  font-weight: 500;
}

.status-pending   { background: hsl(var(--warning) / 0.14);     color: hsl(var(--warning));     border: 1px solid hsl(var(--warning) / 0.3); }
.status-confirmed { background: hsl(var(--info) / 0.12);        color: hsl(var(--info));        border: 1px solid hsl(var(--info) / 0.3); }
.status-completed { background: hsl(var(--success) / 0.12);     color: hsl(var(--success));     border: 1px solid hsl(var(--success) / 0.3); }
.status-cancelled { background: hsl(var(--destructive) / 0.14); color: hsl(var(--destructive)); border: 1px solid hsl(var(--destructive) / 0.3); }
.status-draft     { background: hsl(var(--neutral) / 0.12);     color: hsl(var(--neutral));     border: 1px solid hsl(var(--neutral) / 0.28); }
```

### 3.4 Card Pattern

Reference: [src/app/modules/shared/components/ui/card/card.component.scss](src/app/modules/shared/components/ui/card/card.component.scss)

```scss
.card {
  background: hsl(var(--card));
  color: hsl(var(--card-foreground));
  border: 1px solid hsl(var(--border));
  border-radius: var(--radius);
  box-shadow: var(--shadow-card);
}

// Interactive card (hover lift)
.card-interactive {
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: var(--shadow-elevated);
  }
}
```

### 3.5 Form Input Pattern

Reference: [src/app/modules/shared/components/ui/input/input.component.scss](src/app/modules/shared/components/ui/input/input.component.scss)

```scss
.input {
  background: hsl(var(--input));
  border: 1px solid hsl(var(--border));
  border-radius: calc(var(--radius) - 2px);
  color: hsl(var(--foreground));
  padding: 0.5rem 0.75rem;

  &::placeholder {
    color: hsl(var(--muted-foreground));
  }

  &:focus {
    outline: none;
    border-color: hsl(var(--ring));
    box-shadow: 0 0 0 2px hsl(var(--ring) / 0.2);
  }
}
```

### 3.6 Gradient Text (for headings)

```scss
.gradient-heading {
  background: var(--gradient-accent);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
```

### 3.7 Glass Morphism

Defined as `.glass` utility class in [src/styles.scss](src/styles.scss). Apply directly in HTML, or replicate:

```scss
.glass-panel {
  background: hsl(var(--overlay-foreground-soft));
  backdrop-filter: blur(12px);
  border: 1px solid hsl(var(--overlay-foreground-medium));
}
```

---

## 4. Global Utility Classes

These classes are defined in [src/styles.scss](src/styles.scss) and available everywhere. Apply directly in HTML — no imports needed.

### 4.1 Visual Effects

| Class | Effect |
|-------|--------|
| `.glass` | Glass morphism with blur |
| `.gradient-text` | Gradient-filled text |
| `.gradient-border` | Pseudo-element gradient border |
| `.glow-primary` | Primary color outer glow |
| `.glow-accent` | Accent color outer glow |
| `.card-elevated` | Gradient background + card shadow |
| `.card-interactive` | Elevated card with hover transform |

### 4.2 Buttons

| Class | Style |
|-------|-------|
| `.btn-gradient` | Gradient primary button |
| `.btn-trailing-icon` | Icon + label layout |
| `.input-glow` | Input with focus glow ring |

### 4.3 Navigation

| Class | Use |
|-------|-----|
| `.nav-link` | Muted text, hover → foreground |
| `.nav-link-active` | Primary color + bold |

### 4.4 Status Badges (global)

| Class | Color |
|-------|-------|
| `.badge-status` | Base badge styles |
| `.badge-pending` | Warning orange |
| `.badge-confirmed` | Info blue |
| `.badge-completed` | Success green |
| `.badge-cancelled` | Destructive red |

### 4.5 Shadows

| Class | Strength |
|-------|---------|
| `.shadow-sm` | Subtle |
| `.shadow-md` | Medium |
| `.shadow-lg` | Strong |
| `.shadow-xl` | Very strong |
| `.shadow-card` | Card-specific |
| `.shadow-elevated` | Modals/drawers |

### 4.6 Animations

| Class | Effect |
|-------|--------|
| `.shimmer` | Loading shimmer (1.5s) |
| `.fade-in` | Fade + slide up entrance (0.5s) |
| `.slide-in-right` | Slide from right (0.3s) |
| `.pulse-glow` | Pulsing glow (2s) |
| `.float` | Slow float decoration (6s) |
| `.spin-slow` | Very slow rotation (20s) |

### 4.7 Snackbar / Toast

| Class | Color |
|-------|-------|
| `.snackbar-success` | Green |
| `.snackbar-error` | Red |
| `.snackbar-warning` | Orange |
| `.snackbar-info` | Blue |

---

## 5. Typography

No font imports needed in component SCSS — fonts are globally loaded.

| Font | Use |
|------|-----|
| `'Inter', sans-serif` | Body text, labels, inputs, data |
| `'Space Grotesk', sans-serif` | Headings, titles, display text |

```scss
// Body / data text
.label { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: hsl(var(--muted-foreground)); }

// Heading
.page-title { font-family: 'Space Grotesk', sans-serif; font-size: 1.5rem; font-weight: 700; color: hsl(var(--foreground)); }
```

---

## 6. Angular Material Integration

Material M3 theme is initialized in [src/styles.scss](src/styles.scss) with `mat.$violet-palette`.
Component-level Material overrides are also in [src/styles.scss](src/styles.scss) — over 40 selector overrides.

### 6.1 What Is Already Overridden

- **Buttons** — `.mat-mdc-raised-button[color='primary']` gets gradient + hero shadow automatically
- **Form fields** — `.mat-mdc-form-field`, `.mat-mdc-text-field-wrapper`, outline borders, labels
- **Cards** — `.mat-mdc-card` gets gradient background and custom radius
- **Dialogs** — `.mat-mdc-dialog-surface` gets gradient card surface + elevated shadow
- **Menus / Selects** — dark gradient panel, correct hover and active states
- **Tables** — `.mat-mdc-header-cell` secondary background, `.mat-mdc-row:hover` highlight
- **Tabs** — active label in primary, inactive in muted
- **Tooltips** — card-colored backgrounds

**No additional component SCSS is needed for standard Material usage.** The global overrides handle it.

### 6.2 Material System Tokens

The following Angular Material system tokens are explicitly set in `:root`:

```
--mat-sys-primary           --mat-sys-on-primary
--mat-sys-secondary         --mat-sys-on-secondary
--mat-sys-surface           --mat-sys-on-surface
--mat-sys-surface-container --mat-sys-surface-container-high --mat-sys-surface-container-highest
--mat-sys-surface-variant   --mat-sys-on-surface-variant
--mat-sys-outline           --mat-sys-outline-variant
--mat-sys-error
```

### 6.3 When You Do Need `::ng-deep`

Only use `::ng-deep` when you need to style Material internals that the global overrides in `styles.scss` don't cover for your specific use case. Prefer adding the override to `styles.scss` if it is general enough.

---

## 7. Layout Patterns

### 7.1 Page Container

```scss
.page {
  max-width: var(--page-max-width);
  margin: 0 auto;
  padding: 0 var(--page-padding-x);
}
```

### 7.2 Page Background

```scss
.page-bg {
  background: var(--gradient-page);
  min-height: 100vh;
}
```

### 7.3 Responsive Nav Offset

```scss
.main-content {
  padding-top: var(--nav-height-mobile);

  @media (min-width: 768px) {
    padding-top: var(--nav-height-desktop);
  }
}
```

---

## 8. Implementing a New Feature Module (e.g., Bookings)

Step-by-step checklist when writing SCSS for a new feature:

1. **Do not import palette-bridge.** Use CSS variables from Section 2.
2. **Surface/container:** use `hsl(var(--card))` with `hsl(var(--border))` border and `var(--shadow-card)` shadow.
3. **Text hierarchy:**
   - Primary text → `hsl(var(--foreground))`
   - Secondary / meta text → `hsl(var(--muted-foreground))`
   - Headings → `'Space Grotesk'` font
   - Body → `'Inter'` font
4. **Status indicators:** follow the soft-fill pattern from Section 3.3.
5. **Interactive elements:**
   - Primary actions → `background: hsl(var(--primary)); color: hsl(var(--primary-foreground));`
   - Hover → `background: var(--gradient-primary); box-shadow: var(--shadow-primary);`
   - Destructive → `background: hsl(var(--destructive)); color: hsl(var(--destructive-foreground));`
6. **Material form fields:** the global Material overrides handle default styling. Only add `::ng-deep` overrides if you need something specific.
7. **Animations:** use the global animation classes (`.fade-in`, `.shimmer`, etc.) in the template rather than writing new `@keyframes`.
8. **Check [src/styles.scss](src/styles.scss)** — if you find yourself writing a reusable pattern, put it there instead.

---

## 9. Migration Complete

All 13 previously-legacy component SCSS files have been migrated to CSS custom properties.
`src/styles/_palette-bridge.scss` has been deleted. The codebase is now fully centralized.

Notable fixes applied during migration:
- **`background: white` → `hsl(var(--card))`** in several admin components (dark-theme bug)
- **`darken()`/`lighten()` Sass calls** replaced with pre-computed HSL values
- **`$shadow-md/lg`** inlined as `0 16px 36px -24px hsl(0 0% 0% / 0.6)` / `0 24px 48px -24px hsl(0 0% 0% / 0.7)` since no CSS token exists for them

---

## 10. Reference Components (Modern Pattern — Study These)

| Component | Path | What It Demonstrates |
|-----------|------|---------------------|
| Button | [src/app/modules/shared/components/ui/button/button.component.scss](src/app/modules/shared/components/ui/button/button.component.scss) | 8 button variants, 4 sizes, all CSS vars |
| Card | [src/app/modules/shared/components/ui/card/card.component.scss](src/app/modules/shared/components/ui/card/card.component.scss) | Card pattern, surfaces, elevation |
| Input | [src/app/modules/shared/components/ui/input/input.component.scss](src/app/modules/shared/components/ui/input/input.component.scss) | Form input, focus ring, placeholder |
| Navbar | [src/app/modules/shared/components/site-navbar/site-navbar.component.scss](src/app/modules/shared/components/site-navbar/site-navbar.component.scss) | Complex layout, sticky nav, responsive |
| Hero | [src/app/modules/public/landing/hero/hero.component.scss](src/app/modules/public/landing/hero/hero.component.scss) | Gradient overlays, responsive, animation |
| Status styles | [src/app/modules/admin/orders/admin-orders-status-styles.scss](src/app/modules/admin/orders/admin-orders-status-styles.scss) | Status badge pattern with opacity variants |

---

## 11. Changing the Palette

To change the color palette, update **only one file:**

1. [src/styles.scss](src/styles.scss) — CSS variable definitions in `:root`

All components consume CSS variables at runtime, so a token change propagates everywhere automatically.
Also update the token table in Section 2 of this guide to keep it accurate.

---

## 12. Statistics Snapshot

| Metric | Count |
|--------|-------|
| Total component SCSS files | 61 |
| CSS custom properties in `:root` | 70+ |
| Global utility classes | 40+ |
| Material component overrides | 40+ |
| CSS animations (`@keyframes`) | 6 |
| Components using CSS variables (modern) | 44 (all) |
| Components still on Sass bridge (legacy) | 0 — fully migrated |
| Global stylesheet entry point | 1 (`src/styles.scss`) |
