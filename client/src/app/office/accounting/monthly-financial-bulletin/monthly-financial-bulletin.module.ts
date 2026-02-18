import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MonthlyFinancialBulletinRoutingModule } from './monthly-financial-bulletin-routing.module';
import { MonthlyFinancialBulletinComponent } from './components/monthly-financial-bulletin';
import { FormListModule, FormlyEditModule, MonthlyFinancialBulletinUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        MonthlyFinancialBulletinRoutingModule,
        StickyModule
    ],
    declarations: [
        MonthlyFinancialBulletinComponent
    ],
    providers: [
        MonthlyFinancialBulletinUnitOfWork
    ]
})
export class MonthlyFinancialBulletinModule { }
