import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DATE_FORMATS, MAT_DATE_LOCALE, MatDateFormats } from '@angular/material/core';
import { RouterModule } from '@angular/router';
import { PaginatorBarComponent } from './components/paginator-bar/paginator-bar.component';
import { materialModules } from './material-modules';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { DialogHelperService } from './services/dialog-helper.service';
import { LoadingBarComponent } from './components/loading-bar/loading-bar.component';
import { TableSkeletonComponent } from './components/table-skeleton/table-skeleton.component';
import { SiteNavbarComponent } from './components/site-navbar/site-navbar.component';
import { SiteFooterComponent } from './components/site-footer/site-footer.component';
import { DashboardLayoutComponent } from './components/dashboard-layout/dashboard-layout.component';

// UI Primitives
import { ButtonComponent } from './components/ui/button/button.component';
import { BadgeComponent } from './components/ui/badge/badge.component';
import { CardComponent } from './components/ui/card/card.component';
import { CardHeaderComponent } from './components/ui/card/card-header.component';
import { CardTitleComponent } from './components/ui/card/card-title.component';
import { CardDescriptionComponent } from './components/ui/card/card-description.component';
import { CardContentComponent } from './components/ui/card/card-content.component';
import { CardFooterComponent } from './components/ui/card/card-footer.component';
import { InputComponent } from './components/ui/input/input.component';
import { LabelComponent } from './components/ui/label/label.component';
import { SeparatorComponent } from './components/ui/separator/separator.component';
import { TabsComponent } from './components/ui/tabs/tabs.component';
import { TabsListComponent } from './components/ui/tabs/tabs-list.component';
import { TabsTriggerComponent } from './components/ui/tabs/tabs-trigger.component';
import { TabsContentComponent } from './components/ui/tabs/tabs-content.component';
import { ContainerComponent } from './components/ui/container/container.component';
import { TextareaComponent } from './components/ui/textarea/textarea.component';
import { DetailyDatePipe } from './pipes/detaily-date.pipe';
import { ToastNotificationComponent } from './components/toast-notification/toast-notification.component';

const DETAILY_DATE_FORMATS: MatDateFormats = {
  parse: { dateInput: { year: 'numeric', month: '2-digit', day: '2-digit' } },
  display: {
    dateInput: { year: 'numeric', month: '2-digit', day: '2-digit' },
    monthYearLabel: { year: 'numeric', month: 'short' },
    dateA11yLabel: { year: 'numeric', month: 'long', day: 'numeric' },
    monthYearA11yLabel: { year: 'numeric', month: 'long' },
  },
};

const UI_COMPONENTS = [
  ButtonComponent,
  BadgeComponent,
  CardComponent,
  CardHeaderComponent,
  CardTitleComponent,
  CardDescriptionComponent,
  CardContentComponent,
  CardFooterComponent,
  InputComponent,
  LabelComponent,
  SeparatorComponent,
  TabsComponent,
  TabsListComponent,
  TabsTriggerComponent,
  TabsContentComponent,
  ContainerComponent,
  TextareaComponent,
];

@NgModule({
  declarations: [
    PaginatorBarComponent,
    ConfirmDialogComponent,
    LoadingBarComponent,
    TableSkeletonComponent,
    SiteNavbarComponent,
    SiteFooterComponent,
    DashboardLayoutComponent,
    DetailyDatePipe,
    ToastNotificationComponent,
    ...UI_COMPONENTS,
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    TranslatePipe,
    ...materialModules,
  ],
  providers: [
    DialogHelperService,
    { provide: MAT_DATE_FORMATS, useValue: DETAILY_DATE_FORMATS },
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
  ],
  exports: [
    PaginatorBarComponent,
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    TranslatePipe,
    FormsModule,
    LoadingBarComponent,
    TableSkeletonComponent,
    SiteNavbarComponent,
    SiteFooterComponent,
    DashboardLayoutComponent,
    ...materialModules,
    ...UI_COMPONENTS,
    DetailyDatePipe,
  ],
})
export class SharedModule {}
