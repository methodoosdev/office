import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TaxFactorRoutingModule } from './tax-factor-routing.module';
import { TaxFactorListComponent } from './components/tax-factor-list';
import { TaxFactorEditComponent } from './components/tax-factor-edit';
import { FormlyEditModule, FormListModule, TaxFactorUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TaxFactorRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        TaxFactorListComponent,
        TaxFactorEditComponent
    ],
    providers: [
        TaxFactorUnitOfWork
    ]
})
export class TaxFactorModule { }
