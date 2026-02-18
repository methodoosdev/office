import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CountingDocumentRoutingModule } from './counting-document-routing.module';
import { CountingDocumentComponent } from './components/counting-document';
import { FormListModule, FormlyEditModule, CountingDocumentUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        CountingDocumentRoutingModule
    ],
    declarations: [
        CountingDocumentComponent
    ],
    providers: [
        CountingDocumentUnitOfWork
    ]
})
export class CountingDocumentModule { }
