import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { EmailAccounRoutingModule } from './email-account-routing.module';
import { EmailAccountListComponent } from './components/email-account-list';
import { EmailAccountEditComponent } from './components/email-account-edit';
import { FormlyEditModule, FormListModule, EmailAccountUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        EmailAccounRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        EmailAccountListComponent,
        EmailAccountEditComponent
    ],
    providers: [
        EmailAccountUnitOfWork
    ]
})
export class EmailAccountModule { }
