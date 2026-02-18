import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NbgRoutingModule } from './nbg-routing.module';
import { NbgListComponent } from './components/nbg-list';
import { FormListModule, FormlyEditModule, NbgTransactionsUnitOfWork, OfficeSharedModule } from '@officeNg';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        NbgRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        NbgListComponent,
    ],
    providers: [
        NbgTransactionsUnitOfWork
    ]
})
export class NbgModule { }
