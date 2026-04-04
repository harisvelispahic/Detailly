# UI Architecture

## Purpose

This document explains how the shared UI components, the global theme, and Angular Material fit together in the current codebase.

The goal is accuracy, not idealization.

## High-Level Architecture

The Angular UI is made of three layers:

### Layer 1: Design tokens and global styling

Location:

- `src/styles.scss`

Responsibilities:

- defines Detailly theme tokens with CSS variables
- defines gradients, shadows, radii, layout tokens, and status colors
- applies global typography and scrollbar styles
- overrides Angular Material/MDC surfaces to match the dark Detailly theme

This is the visual foundation of the whole app.

### Layer 2: Shared UI primitives

Location:

- `src/app/modules/shared/components/ui/`

Responsibilities:

- reusable `app-*` selectors
- card composition
- button/badge/container primitives
- light structural tabs system

These components are simple wrappers with SCSS-driven styling.

### Layer 3: Screen-level styling

Locations include:

- `src/app/modules/public/landing/...`
- `src/app/modules/public/public-navbar/...`
- `src/app/modules/auth/...`

Responsibilities:

- hero layout
- landing section grids
- auth layout
- page-specific spacing, imagery, and composition

These files often use Angular Material directly, but styled through the same token system from Layer 1.

## SharedModule as the UI Gateway

`SharedModule` exports:

- all shared `app-*` UI components
- Angular Material modules
- common Angular module dependencies
- `TranslatePipe`

This means most feature modules consume UI through one import surface:

```ts
imports: [SharedModule]
```

Architecturally, that makes `SharedModule` the app's UI gateway rather than the `ui/` folder by itself.

## Component Groups

### Atomic wrappers

- button
- badge
- input
- label
- textarea
- separator
- container

Characteristics:

- small API surface
- style-driven
- no service dependency
- mostly content projection or attribute pass-through

### Structured composition

- card and its 5 subcomponents
- tabs and its 3 subcomponents

Characteristics:

- parent + subcomponent relationship
- spacing and semantics distributed across multiple selectors
- consumers assemble structure in templates

## Runtime Patterns Used

### 1. HostBinding for host classes

Most components use `@HostBinding('class')` to apply base or computed classes to the host element.

Examples:

- `app-button` host gets `app-button-wrapper`
- `app-card` host gets `card-element card-variant-*`
- `app-tabs-trigger` host gets its stateful trigger classes

### 2. Inner real element pattern

Several components style an inner element instead of the host directly.

Examples:

- `app-button` renders an inner `<button>`
- `app-input` renders an inner `<input>`
- `app-textarea` renders an inner `<textarea>`
- `app-badge` renders an inner `<span>`

Implication:

- host classes and inner element classes both matter
- documentation/examples need to reflect projected content and actual attributes correctly

### 3. Variant by CSS class naming convention

Variants are usually expressed via class-name generation, not via a style dictionary at runtime.

Examples:

- `btn-variant-default`
- `badge-variant-success`
- `card-variant-interactive`

Several components still contain unused private helper methods from an older implementation approach. The class-name convention in the live template/HostBinding path is the actual runtime behavior.

### 4. Content projection instead of React-style children props

The shared library relies heavily on `ng-content`.

That is especially important for:

- `app-button`
- the card system
- tabs list/trigger/content
- container

### 5. Manual state wiring for tabs

The tabs system is visual and accessible enough for current use, but it is not a full coordinated state container.

Current model:

- host component owns `activeTab`
- trigger emits `tabSelected`
- consumer sets `activeTab = $event`
- content receives `[isActive]`

`app-tabs` itself is only a light wrapper around projected content.

## Form Architecture Reality

The current shared library does **not** contain form controls integrated with Angular Forms APIs.

Specifically:

- `app-input` is not a `ControlValueAccessor`
- `app-textarea` is not a `ControlValueAccessor`
- neither component emits typed input value changes

This is why much of the real app still uses Angular Material form fields for working forms.

That is not inconsistency by accident. It is the current functional boundary of the shared library.

## Why Angular Material Is Still Present

Angular Material remains heavily used for the following reasons:

- dialog infrastructure already exists
- snackbars already exist
- complex form fields already exist
- selects, menus, tables, paginator, and icon utilities already exist
- many feature screens were built around those APIs before the custom primitives matured

The current strategy is:

- keep Material where it solves real behavior
- override its visual defaults globally so it matches Detailly
- use custom primitives where simple composition is enough

## Theme Architecture

### Token direction

The visual system is now based on Detailly dark HSL tokens.

Examples of categories:

- core surface/text
- accent/status colors
- gradients
- shadows/glows
- radii
- layout width/padding

### Global override direction

`src/styles.scss` now also overrides Material primitives such as:

- buttons
- icon buttons
- form fields
- text fields
- cards
- dialogs
- menus
- selects
- tooltips

That file is effectively the global UI theme engine.

## Public/Auth UI Relationship to the Shared Library

The public landing page and auth screens are not built exclusively from `app-*` components.

They combine:

- token-driven page SCSS
- themed Angular Material controls
- some shared concepts from the UI layer

This means future documentation should not claim:

- "the whole UI is implemented through the shared library"

That would be false.

The accurate statement is:

- "the shared library provides reusable low-level building blocks, while the app also uses themed Angular Material and page-level SCSS"

## Current Strengths

- Centralized dark theme with a clear brand identity
- Reusable button/badge/card/container primitives
- Good token reuse across shared components and page-level styles
- SharedModule gives one import path for most UI needs
- Material visuals are now much closer to the same design language

## Current Weaknesses

- `app-input` / `app-textarea` are not ready for real form replacement
- tabs are manually coordinated
- some components still carry stale helper methods from an older generation path
- older docs previously described a different system than the one in source
- UI usage is split between custom primitives and Material, which requires honest documentation

## Recommended Future Evolution

### Priority 1

- implement `ControlValueAccessor` for `app-input`
- implement `ControlValueAccessor` for `app-textarea`
- document migration guidance away from direct `matInput` where appropriate

### Priority 2

- make `app-tabs` own state and coordinate children automatically
- remove dead helper methods in component classes
- add tests around button, badge, card, and tabs behavior

### Priority 3

- expand the custom library only where it brings real value
- avoid duplicating complex Material features unless there is a strong need

## Architecture Rule of Thumb

When editing UI in this repo, evaluate the task in this order:

1. Is the styling/token change global?
   - edit `src/styles.scss`
2. Is it a reusable primitive concern?
   - edit `src/app/modules/shared/components/ui/...`
3. Is it specific to one screen/section?
   - edit the page-level component SCSS/HTML
4. Does the screen already depend on Angular Material behavior?
   - prefer keeping Material and adjusting theme/styling rather than rebuilding behavior unnecessarily

That rule matches the current codebase and should keep future work consistent.
