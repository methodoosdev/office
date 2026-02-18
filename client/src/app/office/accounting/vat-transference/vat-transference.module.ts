import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VatTransferenceRoutingModule } from './vat-transference-routing.module';
import { VatTransferenceComponent } from './components/vat-transference';
import { FormListModule, FormlyEditModule, VatTransferenceUnitOfWork, OfficeSharedModule } from '@officeNg';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        VatTransferenceRoutingModule
    ],
    declarations: [
        VatTransferenceComponent
    ],
    providers: [
        VatTransferenceUnitOfWork
    ]
})
export class VatTransferenceModule { }
