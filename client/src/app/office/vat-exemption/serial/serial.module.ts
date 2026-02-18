import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VatExemptionSerialRoutingModule } from './serial-routing.module';
import { VatExemptionSerialListComponent } from './components/serial-list';
import { VatExemptionSerialEditComponent } from './components/serial-edit';
import { FormListModule, FormlyEditModule, VatExemptionSerialUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        VatExemptionSerialRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        VatExemptionSerialListComponent,
        VatExemptionSerialEditComponent
    ],
    providers: [
        VatExemptionSerialUnitOfWork
    ]
})
export class VatExemptionSerialModule { }
