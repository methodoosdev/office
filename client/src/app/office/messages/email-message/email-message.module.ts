import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { EmailMessageRoutingModule } from './email-message-routing.module';
import { EmailMessageListComponent } from './components/email-message-list';
import { EmailMessageEditComponent } from './components/email-message-edit';
import { FormlyEditModule, FormListModule, FormListDialogModule, EmailMessageUnitOfWork, OfficeSharedModule, TraderLookupUnitOfWork } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        EmailMessageRoutingModule,
        FormListModule,
        FormlyEditModule,
        FormListDialogModule
    ],
    declarations: [
        EmailMessageListComponent,
        EmailMessageEditComponent
    ],
    providers: [
        EmailMessageUnitOfWork,
        TraderLookupUnitOfWork
    ]
})
export class EmailMessageModule { }
