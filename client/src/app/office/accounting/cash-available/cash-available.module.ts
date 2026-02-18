import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CashAvailableRoutingModule } from './cash-available-routing.module';
import { CashAvailableComponent } from './components/cash-available';
import { FormListModule, FormlyEditModule, CashAvailableUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        CashAvailableRoutingModule
    ],
    declarations: [
        CashAvailableComponent
    ],
    providers: [
        CashAvailableUnitOfWork
    ]
})
export class CashAvailableModule { }
