# Angular UI Library - Visual Component Hierarchy

## Component Dependency Tree

```
┌─────────────────────────────────────────────────────────────────────┐
│                      SHARED MODULE (SharedModule)                   │
│                    Exports all 17 components below                  │
└─────────────────────────────────────────────────────────────────────┘
                                 │
                    ┌────────────┴───────────┐
                    │                        │
         ┌──────────▼──────────┐   ┌─────────▼─────────┐
         │  ATOMIC PRIMITIVES  │   │ COMPOSITE SYSTEMS │
         │   (No Dependencies) │   │ (Built from atoms)│
         └─────────┬──────────┬┘   └────────┬──────────┘
                   │          │             │
      ┌────────────┼──────────┼─────────────┼────────────┐
      │            │          │             │            │
      │            │          │          ┌──┴──┐      ┌──┴──┐
      │            │          │          │CARD │      │TABS │
      │            │          │          └──┬──┘      └──┬──┘
      │            │          │             │            │
      ▼            ▼          ▼          ┌──┴──────┐     │
   BUTTON       BADGE      INPUT       /    │    \     │
                                      │     │     │    │
                              ┌───────▼┐  ┌─▼──┐ ┌─▼──┐│
                              │CARD    │─▶│CARD│ │Card│
                              │HEADER  │  │TYPE│ │FOOT│
                              └────────┘  └────┘ └────┘
                                            │
                                        SEPARATOR
      ▼            ▼          ▼
   LABEL       TEXTAREA   SEPARATOR
      │
      │
      ▼
   CONTAINER
```

## Feature Matrix

### Input Component Features

```
app-input
  ├─ type: string (text, email, password, number, etc.)
  ├─ placeholder: string
  ├─ disabled: boolean
  ├─ required: boolean
  ├─ validation: minLength, maxLength, pattern
  ├─ Styling: Focus ring, border, padding
  └─ Integration: FormControl ready
```

### Button Component Features

```
app-button
  ├─ Variants
  │  ├─ default (primary color)
  │  ├─ destructive (red)
  │  ├─ outline (border only)
  │  ├─ secondary (muted)
  │  ├─ ghost (minimal)
  │  ├─ link (text only)
  │  ├─ gradient (purple gradient)
  │  ├─ hero (large gradient)
  │  ├─ hero-outline (gradient outline)
  │  ├─ glass (frosted glass)
  │  └─ success (green)
  ├─ Sizes
  │  ├─ icon (10x10)
  │  ├─ sm (9x2.5)
  │  ├─ default (10x4)
  │  ├─ lg (12x8)
  │  └─ xl (14x10)
  ├─ States: hover, active, disabled, focus
  └─ Events: clicked (EventEmitter<MouseEvent>)
```

### Card Component System

```
app-card [variant]
  │
  ├─ app-card-header (flex column spacing)
  │  ├─ app-card-title (h3, 2xl, semibold)
  │  └─ app-card-description (p, sm, muted)
  │
  ├─ app-card-content (block, padding)
  │  └─ [any content]
  │
  └─ app-card-footer (flex items-center)
     └─ [button groups, actions]

Variants:
  ├─ default (white bg, subtle shadow)
  ├─ elevated (white bg, medium shadow)
  ├─ interactive (hover effects, cursor pointer)
  ├─ glass (frosted glass effect)
  ├─ gradient (gradient background)
  └─ outline (transparent, border visible)
```

### Tabs Component System

```
app-tabs [activeTab] (activeTabChange)
  │
  ├─ app-tabs-list (flex, rounded bg)
  │  │
  │  ├─ app-tabs-trigger [tabId] [isActive] (tab button)
  │  ├─ app-tabs-trigger [tabId] [isActive]
  │  └─ app-tabs-trigger [tabId] [isActive]
  │
  ├─ app-tabs-content [tabId] [isActive] (display: conditional)
  │  └─ [content]
  │
  ├─ app-tabs-content [tabId] [isActive]
  │  └─ [content]
  │
  └─ app-tabs-content [tabId] [isActive]
     └─ [content]

State Management:
  - Parent tracks activeTab
  - Triggers emit tabSelected events
  - Content shows/hides based on isActive
```

## Size & Spacing Scale

```
Padding/Margin Units:
  └─ 0.25rem = 4px  (text-xs elements)
  └─ 0.5rem  = 8px  (small spacing)
  └─ 1rem    = 16px (default spacing)
  └─ 1.5rem  = 24px (section spacing)
  └─ 2rem    = 32px (large spacing)

Button Sizing:
  ├─ icon:    h-10 w-10 (small circular)
  ├─ sm:      h-9 px-3 (compact)
  ├─ default: h-10 px-4 (standard)
  ├─ lg:      h-12 px-8 (large)
  └─ xl:      h-14 px-10 (extra large)

Container Sizing:
  ├─ sm:   max-w-sm (384px)
  ├─ md:   max-w-md (448px)
  ├─ lg:   max-w-4xl (896px)
  ├─ xl:   max-w-6xl (1280px)
  └─ full: 100% width
```

## Color Semantic Map

```
Status Colors:
  ├─ Primary (Purple)
  │  └─ Actions, CTAs, focus states
  ├─ Destructive (Red)
  │  └─ Errors, delete, danger actions
  ├─ Success (Green)
  │  └─ Confirmation, completed, approved
  ├─ Warning (Amber)
  │  └─ Alerts, caution, pending review
  └─ Info (Blue)
     └─ Information, confirmed, approved

Badge Statuses:
  ├─ pending    (Warning/Amber)
  ├─ confirmed  (Info/Blue)
  ├─ completed  (Success/Green)
  └─ cancelled  (Destructive/Red)

Text Colors:
  ├─ foreground (Primary text)
  ├─ muted-foreground (Secondary text)
  └─ [variant]-foreground (For badges/buttons)

Background Colors:
  ├─ background (Page background)
  ├─ card (Card background)
  ├─ secondary (Muted elements)
  └─ [variant] (For colored elements)
```

## Typography Hierarchy

```
Display (24px, Semibold, font-display)
  └─ Large headings, page titles
     Example: <app-card-title>

Heading (20px, Semibold)
  └─ Section headings
     Example: <h2>

Body (16px, Regular)
  └─ Main content text
     Example: <p>, <app-input>

Small (14px, Regular)
  └─ Secondary text, descriptions
     Example: <app-card-description>

Tiny (12px, Medium)
  └─ Labels, captions
     Example: <app-label>, badge text
```

## Interaction States Hierarchy

```
Component States:
  ├─ Normal (default)
  ├─ Hover (interactive feedback)
  ├─ Focus (keyboard navigation)
  ├─ Active (pressed/selected)
  │  └─ Tab trigger: active state styling
  │  └─ Button: darker/lighter variant
  ├─ Disabled (non-interactive)
  │  └─ opacity-50, cursor-not-allowed
  └─ Error (validation state)
     └─ text-destructive color

Transitions:
  └─ duration-200 to duration-300ms for smooth effects
```

## Data Flow Patterns

### Input with Label (Template)

```
User Input
    │
    └──▶ [formControl]="formControl name"
            │
            └──▶ Component.form.get('fieldName')
                    │
                    └──▶ Reactive Forms manages state
                            │
                            └──▶ Validation shows errors
```

### Button Click (EventEmitter)

```
User Click
    │
    └──▶ button click event
            │
            └──▶ (clicked)="onClickHandler($event)"
                    │
                    └──▶ Component method executes
                            │
                            └──▶ State updates
                                    │
                                    └──▶ Template re-renders
```

### Tabs Navigation

```
User Clicks Tab
    │
    └──▶ app-tabs-trigger (tabSelected)
            │
            └──▶ parent: activeTab = tabId
                    │
                    └──▶ [isActive] binding updates
                            │
                            └──▶ Content display changes
```

## Component Reusability Patterns

### Pattern 1: Slot-Based (Card sub-components)

```html
<app-card>
  <app-card-header><!-- Slot 1 --></app-card-header>
  <app-card-content><!-- Slot 2 --></app-card-content>
  <app-card-footer><!-- Slot 3 --></app-card-footer>
</app-card>
```

### Pattern 2: Variant-Driven (Button, Badge)

```html
<app-button variant="destructive">Delete</app-button>
<app-button variant="success">Confirm</app-button>
<app-button variant="ghost">Cancel</app-button>
```

### Pattern 3: State-Driven (Tabs)

```html
<app-tabs [activeTab]="currentTab" (activeTabChange)="currentTab = $event">
  <!-- Renders based on activeTab state -->
</app-tabs>
```

### Pattern 4: Layout-Based (Container)

```html
<app-container size="lg">
  <!-- Responsive max-width wrapper -->
</app-container>
```

## Usage Flow Diagram

```
Feature Module
    │
    ├─ imports: [SharedModule]
    │
    ├─ Your Component
    │  │
    │  ├─ Template uses: <app-button>, <app-card>, etc.
    │  │
    │  └─ Component TypeScript controls:
    │     ├─ @Input/@Output bindings
    │     ├─ Event handling
    │     └─ State management
    │
    └─ Result: Styled UI with consistent design system
```

## File Navigation Guide

```
src/app/modules/shared/components/ui/
│
├─ button/
│  ├─ button.component.ts ──────▶ Logic & inputs
│  ├─ button.component.html ────▶ Template
│  └─ button.component.scss ────▶ Component styles
│
├─ badge/
│  ├─ badge.component.ts
│  ├─ badge.component.html
│  └─ badge.component.scss
│
├─ card/
│  ├─ card.component.ts ────────▶ Parent component
│  ├─ card-header.component.ts ─▶ Sub-component
│  ├─ card-title.component.ts
│  ├─ card-description.component.ts
│  ├─ card-content.component.ts
│  ├─ card-footer.component.ts
│  └─ [*.html, *.scss for each]
│
├─ tabs/
│  ├─ tabs.component.ts ────────▶ Parent/state
│  ├─ tabs-list.component.ts
│  ├─ tabs-trigger.component.ts
│  ├─ tabs-content.component.ts
│  └─ [*.html, *.scss for each]
│
├─ input/ ──────────────────────▶ Form inputs
├─ label/
├─ textarea/
├─ separator/
├─ container/
│
└─ Documentation/
   ├─ IMPLEMENTATION_SUMMARY.md ▶ Start here!
   ├─ UI_LIBRARY.md ────────────▶ Component reference
   ├─ ARCHITECTURE.md ──────────▶ Technical details
   ├─ EXAMPLES.md ──────────────▶ Copy-paste patterns
   ├─ QUICK_REFERENCE.md ───────▶ Developer guide
   └─ VISUAL_GUIDE.md [this file] ▶ Visual hierarchy
```

---

**Legend:**

- ──▶ Primary use/reference
- ├─ Contains/includes
- └─ Ends branch
- Indentation = hierarchy level

**Document Version:** 1.0
**Created:** 2026-03-29
**Total UI Components:** 17
