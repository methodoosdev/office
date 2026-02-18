import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VatExemptionDocRoutingModule } from './doc-routing.module';
import { VatExemptionDocListComponent } from './components/doc-list';
import { VatExemptionDocEditComponent } from './components/doc-edit';
import { FormListModule, FormlyEditModule, VatExemptionDocUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        VatExemptionDocRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        VatExemptionDocListComponent,
        VatExemptionDocEditComponent
    ],
    providers: [
        VatExemptionDocUnitOfWork
    ]
})
export class VatExemptionDocModule { }
