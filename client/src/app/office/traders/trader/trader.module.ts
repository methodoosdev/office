import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderRoutingModule } from './trader-routing.module';
import { TraderListComponent } from './components/trader-list';
import { TraderEditComponent } from './components/trader-edit';
import { PrimeSharedModule } from '@primeNg';
import {
    FormlyEditNewModule, FormListModule, FormListDetailMappingModule, FormListDetailModule, FormListDialogModule,
    TraderUnitOfWork, EmployeesByTraderUnitOfWork, EmployeeUnitOfWork, TraderKadUnitOfWork, TraderBranchUnitOfWork,
    SrfTraderUnitOfWork, TaxSystemTraderUnitOfWork, CheckFhmPosUnitOfWork, OfficeSharedModule,
    TraderRelationshipUnitOfWork, TraderMembershipUnitOfWork, TraderInfoUnitOfWork,
    TraderRatingUnitOfWork, TraderRatingByTraderUnitOfWork,
    MyDataCredentialsUnitOfWork, TraderBoardMemberUnitOfWork, FinancialObligationUnitOfWork,
    TraderMonthlyBillingUnitOfWork
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PrimeSharedModule,
        TraderRoutingModule,
        FormListModule,
        FormlyEditNewModule,
        FormListDetailModule,
        FormListDetailMappingModule,
        FormListDialogModule
    ],
    declarations: [
        TraderListComponent,
        TraderEditComponent
    ],
    providers: [
        TraderUnitOfWork,
        TraderKadUnitOfWork,
        TraderBranchUnitOfWork,
        EmployeesByTraderUnitOfWork,
        EmployeeUnitOfWork,
        SrfTraderUnitOfWork,
        TaxSystemTraderUnitOfWork,
        CheckFhmPosUnitOfWork,
        TraderRelationshipUnitOfWork,
        TraderMembershipUnitOfWork,
        TraderBoardMemberUnitOfWork,
        TraderInfoUnitOfWork,
        TraderRatingUnitOfWork,
        TraderRatingByTraderUnitOfWork,
        MyDataCredentialsUnitOfWork,
        FinancialObligationUnitOfWork,
        TraderMonthlyBillingUnitOfWork
    ]
})
export class TraderModule { }
