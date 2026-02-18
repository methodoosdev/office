import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AccountingOfficeRoutingModule } from './accounting-office-routing.module';
import { AccountingOfficeEditComponent } from './accounting-office-edit';
import { FormlyEditModule, AccountingOfficeUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        AccountingOfficeRoutingModule,
        FormlyEditModule
    ],
    declarations: [
        AccountingOfficeEditComponent
    ],
    providers: [
        AccountingOfficeUnitOfWork
    ]
})
export class AccountingOfficeModule { }
