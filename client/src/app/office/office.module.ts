import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ListViewModule } from '@progress/kendo-angular-listview';

import { AppLayoutModule } from '@core';
import { BookmarkUnitOfWork, OfficeSharedModule } from '@officeNg';
import { StyleClassModule, ButtonModule } from '@primeNg';

import { OfficeLayoutComponent } from './layout/layout.component';
import { OfficeRoutingModule } from './office-routing.module';
import { OfficeHomeComponent } from './home/home.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ListViewModule,
        OfficeRoutingModule,
        AppLayoutModule,
        OfficeSharedModule,
        StyleClassModule, ButtonModule
    ],
    declarations: [
        OfficeHomeComponent,
        OfficeLayoutComponent,
        //SecurityComponent,
        //CustomerSecurityComponent
    ],
    providers: [
        BookmarkUnitOfWork
    ]
})
export class OfficeModule { }
