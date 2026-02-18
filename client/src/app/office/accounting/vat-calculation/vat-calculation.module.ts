import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VatCalculationRoutingModule } from './vat-calculation-routing.module';
import { VatCalculationComponent } from './components/vat-calculation';
import { FormListModule, FormlyEditModule, VatCalculationUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        VatCalculationRoutingModule
    ],
    declarations: [
        VatCalculationComponent
    ],
    providers: [
        VatCalculationUnitOfWork
    ]
})
export class VatCalculationModule { }
