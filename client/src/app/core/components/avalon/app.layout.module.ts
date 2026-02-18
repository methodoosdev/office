import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppConfigModule } from './config/app.config.module';
import { AppLayoutComponent } from './app.layout.component';
import { AppBreadcrumbComponent } from './app.breadcrumb.component';
import { AppMenuProfileComponent } from './app.menuprofile.component';
import { AppTopbarComponent } from './app.topbar.component';
import { AppRightMenuComponent } from './app.rightmenu.component';
import { AppMenuComponent } from './app.menu.component';
import { RouterModule } from '@angular/router';
import { AppSidebarComponent } from './app.sidebar.component';
import { AppFooterComponent } from './app.footer.component';
import { AppMenuitemComponent } from './app.menuitem.component';
import {
    BadgeModule, ButtonModule, InputSwitchModule, InputTextModule/*, MegaMenuModule*/, MenuModule, RadioButtonModule,
    RippleModule,
    SidebarModule, StyleClassModule, TooltipModule
} from '@primeNg';
import { CommonModule } from '@angular/common';

@NgModule({
    declarations: [
        AppLayoutComponent,
        AppBreadcrumbComponent,
        AppMenuProfileComponent,
        AppTopbarComponent,
        AppRightMenuComponent,
        AppMenuComponent,
        AppSidebarComponent,
        AppFooterComponent,
        AppMenuitemComponent
    ],
    exports: [
        AppLayoutComponent,
        AppBreadcrumbComponent,
        AppMenuProfileComponent,
        AppTopbarComponent,
        AppRightMenuComponent,
        AppMenuComponent,
        AppSidebarComponent,
        AppFooterComponent,
        AppMenuitemComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        StyleClassModule,
        InputTextModule,
        SidebarModule,
        BadgeModule,
        RadioButtonModule,
        InputSwitchModule,
        TooltipModule,
        //MegaMenuModule,
        RippleModule,
        ButtonModule,
        MenuModule,
        AppConfigModule
    ]
})
export class AppLayoutModule { }
