import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MonthlyBCategoryBulletinRoutingModule } from './monthly-bCategory-routing.module';
import { MonthlyBCategoryBulletinComponent } from './components/monthly-bCategory';
import { FormListModule, FormlyEditModule, OfficeSharedModule, StickyModule, MonthlyBCategoryBulletinUnitOfWork } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        MonthlyBCategoryBulletinRoutingModule,
        StickyModule
    ],
    declarations: [
        MonthlyBCategoryBulletinComponent
    ],
    providers: [
        MonthlyBCategoryBulletinUnitOfWork
    ]
})
export class MonthlyBCategoryBulletinModule { }
