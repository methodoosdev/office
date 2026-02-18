import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PayoffLiabilitiesRoutingModule } from './payoff-liabilities-routing.module';
import { PayoffLiabilitiesComponent } from './components/payoff-liabilities';
import { FormListModule, FormlyEditModule, PayoffLiabilitiesUnitOfWork, OfficeSharedModule } from '@officeNg';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        PayoffLiabilitiesRoutingModule
    ],
    declarations: [
        PayoffLiabilitiesComponent
    ],
    providers: [
        PayoffLiabilitiesUnitOfWork
    ]
})
export class PayoffLiabilitiesModule { }
