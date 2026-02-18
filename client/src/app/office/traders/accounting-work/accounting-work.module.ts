import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AccountingWorkRoutingModule } from './accounting-work-routing.module';
import { AccountingWorkListComponent } from './components/accounting-work-list';
import { AccountingWorkEditComponent } from './components/accounting-work-edit';
import { FormListModule, FormlyEditModule, AccountingWorkUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        AccountingWorkRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        AccountingWorkListComponent,
        AccountingWorkEditComponent
    ],
    providers: [
        AccountingWorkUnitOfWork
    ]
})
export class AccountingWorkModule { }
