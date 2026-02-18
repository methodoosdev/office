import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderChargeRoutingModule } from './trader-charge-routing.module';
import { TraderChargeComponent } from './components/trader-charge';
import { FormListModule, FormlyEditModule, TraderChargeUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        TraderChargeRoutingModule
    ],
    declarations: [
        TraderChargeComponent
    ],
    providers: [
        TraderChargeUnitOfWork
    ]
})
export class TraderChargeModule { }
