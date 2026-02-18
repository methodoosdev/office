import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FormListModule, FormlyEditModule, OfficeSharedModule } from '@officeNg';
import { ESendUnitOfWork } from '@officeNg';

import { ESendRoutingModule } from './eSend-routing.module';
import { ESendComponent } from './components/eSend';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        ESendRoutingModule
    ],
    declarations: [
        ESendComponent
    ],
    providers: [
        ESendUnitOfWork
    ]
})
export class ESendModule { }
