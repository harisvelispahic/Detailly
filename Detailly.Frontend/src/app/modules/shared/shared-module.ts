import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FitPaginatorBarComponent } from './components/fit-paginator-bar/fit-paginator-bar.component';
import { materialModules } from './material-modules';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { FitConfirmDialogComponent } from './components/fit-confirm-dialog/fit-confirm-dialog.component';
import { DialogHelperService } from './services/dialog-helper.service';
import { FitLoadingBarComponent } from './components/fit-loading-bar/fit-loading-bar.component';
import { FitTableSkeletonComponent } from './components/fit-table-skeleton/fit-table-skeleton.component';
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
    FitPaginatorBarComponent,
    FitConfirmDialogComponent,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    SiteNavbarComponent,
    SiteFooterComponent,
    DashboardLayoutComponent,
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
  providers: [DialogHelperService],
  exports: [
    FitPaginatorBarComponent,
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    TranslatePipe,
    FormsModule,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    SiteNavbarComponent,
    SiteFooterComponent,
    DashboardLayoutComponent,
    ...materialModules,
    ...UI_COMPONENTS,
  ],
})
export class SharedModule {}
