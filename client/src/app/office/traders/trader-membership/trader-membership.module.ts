import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderMembershipRoutingModule } from './trader-membership-routing.module';
import { TraderMembershipEditComponent } from './components/trader-membership-edit';
import { FormListModule, FormlyEditNewModule, TraderMembershipUnitOfWork, OfficeSharedModule } from '@officeNg';
//bugathina
@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderMembershipRoutingModule,
        FormListModule,
        FormlyEditNewModule
    ],
    declarations: [
        TraderMembershipEditComponent
    ],
    providers: [
        TraderMembershipUnitOfWork
    ]
})
export class TraderMembershipModule { }
