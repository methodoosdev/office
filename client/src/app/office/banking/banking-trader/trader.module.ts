import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { BankingTraderRoutingModule } from './trader-routing.module';
import { BankingTraderListComponent } from './components/trader-list';
import { BankingTraderEditComponent } from './components/trader-edit';
import {
    FormListModule, FormlyEditModule,
    BankingTraderUnitOfWork, AvailableBankUnitOfWork, UserConnectionBankUnitOfWork, AccountListUnitOfWork, CardListItemUnitOfWork,
    FormlyEditDialogModule,
    OfficeSharedModule, FormListDetailDialogModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        BankingTraderRoutingModule,
        FormListModule,
        FormlyEditModule,
        FormListDetailDialogModule,
        FormlyEditDialogModule
    ],
    declarations: [
        BankingTraderListComponent,
        BankingTraderEditComponent
    ],
    providers: [
        BankingTraderUnitOfWork,
        AvailableBankUnitOfWork,
        UserConnectionBankUnitOfWork,
        AccountListUnitOfWork,
        CardListItemUnitOfWork
    ]
})
export class BankingTraderModule { }
