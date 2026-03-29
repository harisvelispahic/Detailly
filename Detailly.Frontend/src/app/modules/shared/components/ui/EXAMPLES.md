# UI Components - Usage Examples

Quick reference guide with copy-paste ready code examples.

## Common Form Patterns

### Basic Contact Form

```html
<app-card variant="default">
  <app-card-header>
    <app-card-title>Contact Us</app-card-title>
    <app-card-description>We'll get back to you soon</app-card-description>
  </app-card-header>

  <app-card-content class="space-y-4">
    <div class="form-group">
      <app-label for="name" [required]="true">Full Name</app-label>
      <app-input id="name" type="text" placeholder="John Doe" [formControl]="form.get('name')">
      </app-input>
      <span *ngIf="form.get('name')?.errors?.required" class="text-sm text-destructive mt-1">
        Name is required
      </span>
    </div>

    <div class="form-group">
      <app-label for="email" [required]="true">Email Address</app-label>
      <app-input
        id="email"
        type="email"
        placeholder="john@example.com"
        [formControl]="form.get('email')"
      >
      </app-input>
      <span *ngIf="form.get('email')?.errors?.required" class="text-sm text-destructive mt-1">
        Email is required
      </span>
    </div>

    <div class="form-group">
      <app-label for="message">Message</app-label>
      <app-textarea
        id="message"
        placeholder="Your message..."
        [rows]="5"
        [formControl]="form.get('message')"
      >
      </app-textarea>
    </div>
  </app-card-content>

  <app-card-footer class="flex gap-3 justify-end">
    <app-button variant="outline" (clicked)="onCancel()"> Cancel </app-button>
    <app-button variant="default" [disabled]="!form.valid" (clicked)="onSubmit()">
      Send Message
    </app-button>
  </app-card-footer>
</app-card>
```

### Login Form with Validation

```html
<app-container size="sm" class="flex items-center justify-center min-h-screen">
  <app-card variant="elevated">
    <app-card-header>
      <app-card-title>Sign In</app-card-title>
      <app-card-description>Enter your credentials</app-card-description>
    </app-card-header>

    <app-card-content class="space-y-4">
      <div class="form-group">
        <app-label for="email">Email</app-label>
        <app-input
          id="email"
          type="email"
          [formControl]="form.get('email')"
          placeholder="email@example.com"
        >
        </app-input>
      </div>

      <div class="form-group">
        <app-label for="password">Password</app-label>
        <app-input id="password" type="password" [formControl]="form.get('password')"> </app-input>
      </div>

      <!-- Error Alert -->
      <div
        *ngIf="errorMessage"
        class="bg-destructive/10 text-destructive px-4 py-2 rounded-md text-sm"
      >
        {{ errorMessage }}
      </div>
    </app-card-content>

    <app-card-footer>
      <app-button variant="default" class="w-full" [disabled]="isLoading" (clicked)="onLogin()">
        {{ isLoading ? 'Signing in...' : 'Sign In' }}
      </app-button>
    </app-card-footer>
  </app-card>
</app-container>
```

## Card Variations

### Product Card (Interactive)

```html
<app-card variant="interactive" class="cursor-pointer" (click)="onProductClick(product)">
  <app-card-header>
    <img
      [src]="product.image"
      alt="{{ product.name }}"
      class="w-full h-48 object-cover rounded-lg mb-4"
    />
    <app-card-title>{{ product.name }}</app-card-title>
  </app-card-header>

  <app-card-content>
    <app-card-description class="mb-2"> {{ product.description }} </app-card-description>
    <div class="flex items-center justify-between">
      <span class="text-xl font-semibold">${{ product.price }}</span>
      <app-badge variant="success">In Stock</app-badge>
    </div>
  </app-card-content>

  <app-card-footer class="gap-2">
    <app-button variant="outline" size="sm">View Details</app-button>
    <app-button variant="default" size="sm">Add to Cart</app-button>
  </app-card-footer>
</app-card>
```

### Stats Card

```html
<app-card variant="gradient">
  <app-card-content class="text-center py-6">
    <div class="text-4xl font-bold text-primary mb-2">{{ stats.value }}</div>
    <p class="text-muted-foreground">{{ stats.label }}</p>
    <app-separator class="my-4"></app-separator>
    <p class="text-sm"><span class="text-success">↑ 12%</span> from last month</p>
  </app-card-content>
</app-card>
```

### Loading Card

```html
<app-card variant="glass">
  <app-card-content class="flex items-center justify-center py-8">
    <div class="text-center">
      <div
        class="animate-spin rounded-full h-12 w-12 border-4 border-primary border-t-transparent mx-auto mb-4"
      ></div>
      <p class="text-muted-foreground">Loading...</p>
    </div>
  </app-card-content>
</app-card>
```

## Button Usage Patterns

### Primary Actions

```html
<!-- Default Primary Button -->
<app-button (clicked)="onSave()">Save Changes</app-button>

<!-- Large Hero Button -->
<app-button variant="hero" size="lg" (clicked)="onStartNow()"> Get Started Now </app-button>

<!-- With Icon -->
<app-button size="icon" (clicked)="onToggleMenu()">
  <mat-icon>menu</mat-icon>
</app-button>
```

### Secondary Actions

```html
<!-- Outline Button -->
<app-button variant="outline" (clicked)="onCancel()"> Cancel </app-button>

<!-- Ghost Button -->
<app-button variant="ghost" size="sm" (clicked)="onLearnMore()"> Learn More </app-button>

<!-- Link Button -->
<app-button variant="link" (clicked)="onForgotPassword()"> Forgot Password? </app-button>
```

### Danger Actions

```html
<!-- Delete Button -->
<app-button variant="destructive" (clicked)="onDelete()"> Delete </app-button>

<!-- Destructive Outline -->
<app-button variant="destructive" class="outline" (clicked)="onRemove()"> Remove </app-button>
```

### Success Actions

```html
<!-- Success Button -->
<app-button variant="success" (clicked)="onComplete()"> Mark Complete </app-button>

<!-- Success Badge + Button -->
<div class="flex items-center gap-2">
  <app-badge variant="completed">Completed</app-badge>
  <app-button variant="success" size="sm">Verified</app-button>
</div>
```

### Button Groups

```html
<div class="flex gap-2">
  <app-button variant="outline" size="sm">Edit</app-button>
  <app-button variant="outline" size="sm">Share</app-button>
  <app-button variant="outline" size="sm">Delete</app-button>
</div>
```

## Badge Status Displays

### Order Status

```html
<div class="flex items-center gap-2">
  <span>Status:</span>
  <app-badge *ngIf="order.status === 'pending'" variant="pending"> Pending Review </app-badge>
  <app-badge *ngIf="order.status === 'confirmed'" variant="confirmed"> Confirmed </app-badge>
  <app-badge *ngIf="order.status === 'completed'" variant="completed"> Completed </app-badge>
  <app-badge *ngIf="order.status === 'cancelled'" variant="cancelled"> Cancelled </app-badge>
</div>
```

### Multi-Badge Display

```html
<div class="flex flex-wrap gap-2">
  <app-badge variant="default">Angular</app-badge>
  <app-badge variant="secondary">TypeScript</app-badge>
  <app-badge variant="success">Active</app-badge>
  <app-badge variant="warning">In Progress</app-badge>
  <app-badge variant="destructive">Critical</app-badge>
</div>
```

## Tabs Implementation

### Simple Navigation Tabs

```typescript
// Component TypeScript
export class ProfileComponent {
  activeTab = 'profile';

  onTabSelect(tabId: string): void {
    this.activeTab = tabId;
  }
}
```

```html
<!-- Component HTML -->
<app-tabs [activeTab]="activeTab" (activeTabChange)="onTabSelect($event)">
  <app-tabs-list>
    <app-tabs-trigger
      tabId="profile"
      [isActive]="activeTab === 'profile'"
      (tabSelected)="onTabSelect($event)"
    >
      Profile
    </app-tabs-trigger>
    <app-tabs-trigger
      tabId="settings"
      [isActive]="activeTab === 'settings'"
      (tabSelected)="onTabSelect($event)"
    >
      Settings
    </app-tabs-trigger>
    <app-tabs-trigger
      tabId="security"
      [isActive]="activeTab === 'security'"
      (tabSelected)="onTabSelect($event)"
    >
      Security
    </app-tabs-trigger>
  </app-tabs-list>

  <app-tabs-content tabId="profile" [isActive]="activeTab === 'profile'">
    <app-card>
      <app-card-header>
        <app-card-title>Profile Information</app-card-title>
      </app-card-header>
      <app-card-content>
        <!-- Profile content -->
      </app-card-content>
    </app-card>
  </app-tabs-content>

  <app-tabs-content tabId="settings" [isActive]="activeTab === 'settings'">
    <app-card>
      <app-card-header>
        <app-card-title>Settings</app-card-title>
      </app-card-header>
      <app-card-content>
        <!-- Settings content -->
      </app-card-content>
    </app-card>
  </app-tabs-content>

  <app-tabs-content tabId="security" [isActive]="activeTab === 'security'">
    <app-card>
      <app-card-header>
        <app-card-title>Security Settings</app-card-title>
      </app-card-header>
      <app-card-content>
        <!-- Security content -->
      </app-card-content>
    </app-card>
  </app-tabs-content>
</app-tabs>
```

## Layout Patterns

### Hero Section

```html
<div class="bg-gradient-to-r from-purple-600 via-violet-600 to-indigo-600 py-16">
  <app-container>
    <div class="text-center text-white">
      <h1 class="text-4xl font-bold mb-4">Welcome</h1>
      <p class="text-lg mb-8 text-white/80">Start your journey today</p>
      <app-button variant="hero" size="lg">Get Started</app-button>
    </div>
  </app-container>
</div>
```

### Grid Layout

```html
<app-container>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
    <app-card *ngFor="let item of items" variant="elevated">
      <app-card-header>
        <app-card-title>{{ item.title }}</app-card-title>
      </app-card-header>
      <app-card-content> {{ item.description }} </app-card-content>
      <app-card-footer>
        <app-button variant="outline" size="sm">View More</app-button>
      </app-card-footer>
    </app-card>
  </div>
</app-container>
```

### Two-Column Layout

```html
<app-container size="xl">
  <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 my-8">
    <!-- Sidebar -->
    <aside class="lg:col-span-1">
      <app-card>
        <app-card-header>
          <app-card-title>Filters</app-card-title>
        </app-card-header>
        <app-card-content class="space-y-4">
          <!-- Filter options -->
        </app-card-content>
      </app-card>
    </aside>

    <!-- Main Content -->
    <main class="lg:col-span-2">
      <div class="space-y-4">
        <app-card *ngFor="let item of filteredItems" variant="interactive">
          <!-- Item content -->
        </app-card>
      </div>
    </main>
  </div>
</app-container>
```

## Common UI Patterns

### Empty State

```html
<app-card variant="outline" class="text-center py-12">
  <div class="mb-4">
    <mat-icon class="text-6xl text-muted-foreground">inbox</mat-icon>
  </div>
  <app-card-title>No items found</app-card-title>
  <app-card-description class="mt-2"> Try adjusting your filters or search </app-card-description>
  <app-button variant="outline" size="sm" class="mt-4">Clear Filters</app-button>
</app-card>
```

### Success Message

```html
<div
  class="bg-success/10 border border-success text-success px-4 py-3 rounded-lg flex items-start gap-3"
>
  <mat-icon class="mt-0.5">check_circle</mat-icon>
  <div>
    <p class="font-semibold">Success!</p>
    <p class="text-sm">Your changes have been saved</p>
  </div>
</div>
```

### Error Message

```html
<div
  class="bg-destructive/10 border border-destructive text-destructive px-4 py-3 rounded-lg flex items-start gap-3"
>
  <mat-icon class="mt-0.5">error</mat-icon>
  <div>
    <p class="font-semibold">Error</p>
    <p class="text-sm">{{ error.message }}</p>
  </div>
</div>
```

### Loading Skeleton

```html
<div class="space-y-4">
  <div class="bg-muted h-4 rounded w-3/4 animate-pulse"></div>
  <div class="bg-muted h-4 rounded w-full animate-pulse"></div>
  <div class="bg-muted h-4 rounded w-5/6 animate-pulse"></div>
</div>
```

## Styling Tips

### Spacing Classes (Tailwind)

```html
<!-- Margin -->
<div class="m-4">Margin all sides</div>
<div class="mt-4 mb-2">Top and bottom margin</div>

<!-- Padding -->
<div class="p-6">Padding all sides</div>
<div class="px-4 py-2">X and Y padding</div>

<!-- Gap (flex/grid) -->
<div class="flex gap-4">
  <app-button>Button 1</app-button>
  <app-button>Button 2</app-button>
</div>
```

### Responsive Classes

```html
<!-- Mobile-first approach -->
<div class="text-base md:text-lg lg:text-xl">Responsive text sizing</div>

<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
  <!-- 1 column on mobile, 2 on tablet, 3 on desktop -->
</div>
```

### Color Utilities

```html
<!-- Text colors -->
<p class="text-primary">Primary text</p>
<p class="text-muted-foreground">Muted text</p>

<!-- Background colors -->
<div class="bg-secondary">Secondary background</div>

<!-- Border colors -->
<div class="border border-border">Bordered element</div>
```
