# Angular UI Component Library

## Overview

A reusable, non-standalone Angular UI component library built from React reference components. Follows Material Design principles with Tailwind CSS styling and provides atomic, composable UI primitives.

## Component Organization

### Directory Structure

```
src/app/modules/shared/components/ui/
├── button/
│   ├── button.component.ts
│   ├── button.component.html
│   └── button.component.scss
├── badge/
├── card/
│   ├── card.component.ts (Parent)
│   ├── card-header.component.ts
│   ├── card-title.component.ts
│   ├── card-description.component.ts
│   ├── card-content.component.ts
│   └── card-footer.component.ts
├── input/
├── label/
├── separator/
├── tabs/
│   ├── tabs.component.ts (Parent)
│   ├── tabs-list.component.ts
│   ├── tabs-trigger.component.ts
│   └── tabs-content.component.ts
├── container/
└── textarea/
```

## Component Reference

### Low-Level Reusable Primitives

#### 1. **Button** (`app-button`)

Primary interactive element with multiple variants and sizes.

**Inputs:**

- `variant`: `'default' | 'destructive' | 'outline' | 'secondary' | 'ghost' | 'link' | 'gradient' | 'hero' | 'hero-outline' | 'glass' | 'success'` (default: `'default'`)
- `size`: `'default' | 'sm' | 'lg' | 'xl' | 'icon'` (default: `'default'`)
- `disabled`: `boolean` (default: `false`)
- `type`: `'button' | 'submit' | 'reset'` (default: `'button'`)
- `ariaLabel`: `string` (optional)

**Outputs:**

- `clicked: EventEmitter<MouseEvent>`

**Example:**

```html
<app-button variant="primary" size="lg" (clicked)="handleClick($event)"> Click Me </app-button>

<app-button variant="hero" size="icon">
  <mat-icon>add</mat-icon>
</app-button>
```

#### 2. **Badge** (`app-badge`)

Small label with semantic variants for status/category indication.

**Inputs:**

- `variant`: `'default' | 'secondary' | 'destructive' | 'outline' | 'success' | 'warning' | 'info' | 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'glass'` (default: `'default'`)

**Example:**

```html
<app-badge variant="success">Active</app-badge>
<app-badge variant="pending">Pending</app-badge>
<app-badge variant="destructive">Error</app-badge>
```

#### 3. **Card** (`app-card`) - Composite Component

Container with elevation and variants. Compose with sub-components.

**Inputs:**

- `variant`: `'default' | 'elevated' | 'interactive' | 'glass' | 'gradient' | 'outline'` (default: `'default'`)

**Sub-Components:**

- `app-card-header`: Container for header section
- `app-card-title`: Heading element
- `app-card-description`: Descriptive text
- `app-card-content`: Main content area
- `app-card-footer`: Footer section

**Example:**

```html
<app-card variant="elevated">
  <app-card-header>
    <app-card-title>Payment Details</app-card-title>
    <app-card-description>Enter your payment information</app-card-description>
  </app-card-header>

  <app-card-content>
    <app-input placeholder="Card number"></app-input>
  </app-card-content>

  <app-card-footer>
    <app-button>Submit</app-button>
  </app-card-footer>
</app-card>
```

#### 4. **Input** (`app-input`)

Text input field with consistent styling and accessibility.

**Inputs:**

- `type`: `string` (default: `'text'`)
- `placeholder`: `string`
- `disabled`: `boolean`
- `required`: `boolean`
- `id`: `string` (optional)
- `name`: `string` (optional)
- `value`: `string`
- `minLength`: `number` (optional)
- `maxLength`: `number` (optional)
- `pattern`: `string` (optional)

**Example:**

```html
<app-label for="email">Email Address</app-label>
<app-input id="email" type="email" placeholder="user@example.com" required> </app-input>
```

#### 5. **Label** (`app-label`)

Form label with optional required indicator.

**Inputs:**

- `for`: `string` (optional) - Associates with input id
- `required`: `boolean` (default: `false`)

**Example:**

```html
<app-label for="name" [required]="true">Name</app-label> <app-input id="name"></app-input>
```

#### 6. **Separator** (`app-separator`)

Divider line (horizontal or vertical).

**Inputs:**

- `orientation`: `'horizontal' | 'vertical'` (default: `'horizontal'`)
- `decorative`: `boolean` (default: `true`)

**Example:**

```html
<div>Section 1</div>
<app-separator></app-separator>
<div>Section 2</div>

<!-- Vertical separator -->
<app-separator orientation="vertical"></app-separator>
```

#### 7. **Textarea** (`app-textarea`)

Multi-line text input field.

**Inputs:**

- `placeholder`: `string`
- `disabled`: `boolean`
- `required`: `boolean`
- `id`: `string` (optional)
- `name`: `string` (optional)
- `value`: `string`
- `minLength`: `number` (optional)
- `maxLength`: `number` (optional)
- `rows`: `number` (default: `4`)

**Example:**

```html
<app-label for="message">Message</app-label>
<app-textarea id="message" placeholder="Enter your message..." [rows]="6"> </app-textarea>
```

#### 8. **Tabs** (`app-tabs`) - Composite Component

Tabbed interface for organizing content.

**Inputs:**

- `activeTab`: `string` - ID of currently active tab
- `activeTabChange: EventEmitter<string>` - Emits when tab changes

**Sub-Components:**

- `app-tabs-list`: Container for tabs
- `app-tabs-trigger`: Tab button (receives `tabId`, `isActive`, `disabled`)
- `app-tabs-content`: Content panel (receives `tabId`, `isActive`)

**Example:**

```html
<app-tabs [activeTab]="activeTab" (activeTabChange)="activeTab = $event">
  <app-tabs-list>
    <app-tabs-trigger
      tabId="tab1"
      [isActive]="activeTab === 'tab1'"
      (tabSelected)="onTabSelect($event)"
    >
      Dashboard
    </app-tabs-trigger>
    <app-tabs-trigger
      tabId="tab2"
      [isActive]="activeTab === 'tab2'"
      (tabSelected)="onTabSelect($event)"
    >
      Settings
    </app-tabs-trigger>
  </app-tabs-list>

  <app-tabs-content tabId="tab1" [isActive]="activeTab === 'tab1'">
    <!-- Dashboard content -->
  </app-tabs-content>
  <app-tabs-content tabId="tab2" [isActive]="activeTab === 'tab2'">
    <!-- Settings content -->
  </app-tabs-content>
</app-tabs>
```

#### 9. **Container** (`app-container`)

Responsive wrapper with max-width constraints.

**Inputs:**

- `size`: `'sm' | 'md' | 'lg' | 'xl' | 'full'` (default: `'lg'`)
- `padding`: `boolean` (default: `true`)

**Example:**

```html
<app-container size="xl">
  <h1>Page Title</h1>
  <p>Content within responsive container</p>
</app-container>
```

## Design System

### Color Tokens

The components use CSS variables for theming:

- `--color-primary`: Primary action color
- `--color-secondary`: Secondary element color
- `--color-destructive`: Error/delete actions
- `--color-success`: Success states
- `--color-warning`: Warning states
- `--color-info`: Information states
- `--color-foreground`: Text color
- `--color-background`: Page background
- `--color-card`: Card background
- `--color-muted-foreground`: Muted text

### Spacing Scale

Consistent padding/margin using rem units:

- `0.25rem (4px)`, `0.5rem (8px)`, `1rem (16px)`, `1.5rem (24px)`, `2rem (32px)`

### Typography

- **Display**: Large headings (24px, semibold)
- **Heading**: Card titles (20px, semibold)
- **Body**: Standard text (16px, regular)
- **Small**: Captions (14px, regular)
- **Tiny**: Labels (12px, medium)

### Effects

- **Shadows**: sm, md, lg, xl with subtle black opacity
- **Blur**: backdrop-blur for glass effect
- **Transitions**: 200-300ms duration for smooth interactions
- **Borders**: Rounded corners (md, lg, xl radius)

## Usage Examples

### Form Layout

```html
<app-card>
  <app-card-header>
    <app-card-title>Create Account</app-card-title>
  </app-card-header>

  <app-card-content class="space-y-4">
    <div>
      <app-label for="name" [required]="true">Full Name</app-label>
      <app-input id="name" placeholder="John Doe"></app-input>
    </div>

    <div>
      <app-label for="email" [required]="true">Email</app-label>
      <app-input id="email" type="email" placeholder="john@example.com"></app-input>
    </div>

    <div>
      <app-label for="message">Message</app-label>
      <app-textarea id="message" placeholder="Your message..."></app-textarea>
    </div>
  </app-card-content>

  <app-card-footer class="flex gap-2 justify-end">
    <app-button variant="outline">Cancel</app-button>
    <app-button variant="default">Submit</app-button>
  </app-card-footer>
</app-card>
```

### Status Display

```html
<div class="flex items-center gap-2">
  <span>Order Status:</span>
  <app-badge *ngIf="order.status === 'confirmed'" variant="confirmed"> Confirmed </app-badge>
  <app-badge *ngIf="order.status === 'pending'" variant="pending"> Pending </app-badge>
  <app-badge *ngIf="order.status === 'completed'" variant="completed"> Completed </app-badge>
</div>
```

## Best Practices

1. **Component Composition**: Use atomic components to build larger features
2. **Semantic HTML**: Components emit proper HTML elements (button, input, label, etc.)
3. **Accessibility**: All components include ARIA attributes
4. **Type Safety**: Use provided type unions for variant/size props
5. **Theming**: Customize via CSS variables in global styles
6. **Two-Way Binding**: Use `[(ngModel)]` with inputs, or `@Output()` with buttons
7. **Content Projection**: Use `<ng-content>` for flexible slot-based composition

## Import in Your Modules

The components are already exported in `SharedModule`. Simply:

```typescript
import { SharedModule } from '@app/modules/shared/shared-module';

@NgModule({
  imports: [SharedModule],
  // ...
})
export class MyFeatureModule {}
```

## Future Enhancements

Planned components for next batch:

- Checkbox / Radio
- Select / Dropdown
- Toggle
- Dialog / Modal
- Toast / Alert
- Breadcrumb
- Pagination
- Skeleton / Loading states
- Tooltip
- Popover
- Accordion
- Carousel
- Avatar
- Progress bar

## Customization

### Override Variant Styles

Extend component classes in your SCSS:

```scss
// In your component styles
.app-button {
  &.my-custom-variant {
    // Custom styles
  }
}
```

### CSS Variables

Define in `styles.scss` for global theming:

```scss
:root {
  --color-primary: #6d28d9;
  --color-secondary: #e5e7eb;
  // ... other tokens
}
```
