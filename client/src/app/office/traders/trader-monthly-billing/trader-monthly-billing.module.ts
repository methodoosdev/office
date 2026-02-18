import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderMonthlyBillingRoutingModule } from './trader-monthly-billing-routing.module';
import { TraderMonthlyBillingEditComponent } from './components/trader-monthly-billing-edit';
import { FormListModule, FormlyEditNewModule, TraderMonthlyBillingUnitOfWork, OfficeSharedModule } from '@officeNg';
//bugathina
@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderMonthlyBillingRoutingModule,
        FormListModule,
        FormlyEditNewModule
    ],
    declarations: [
        TraderMonthlyBillingEditComponent
    ],
    providers: [
        TraderMonthlyBillingUnitOfWork
    ]
})
export class TraderMonthlyBillingModule { }
