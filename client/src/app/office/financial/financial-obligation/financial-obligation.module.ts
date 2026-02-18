import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FinancialObligationRoutingModule } from './financial-obligation-routing.module';
import { FinancialObligationListComponent } from './components/financial-obligation-list';
import { FinancialObligationEditComponent } from './components/financial-obligation-edit';
import { PrimeSharedModule } from '@primeNg';
import { FormlyEditModule, FormListModule, FinancialObligationUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        PrimeSharedModule,
        OfficeSharedModule,
        FinancialObligationRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        FinancialObligationListComponent,
        FinancialObligationEditComponent
    ],
    providers: [
        FinancialObligationUnitOfWork
    ]
})
export class FinancialObligationModule { }
