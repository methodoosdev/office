import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IntertemporalCRoutingModule } from './intertemporal-c-routing.module';
import { IntertemporalCComponent } from './components/intertemporal-c';
import { FormListModule, FormlyEditModule, IntertemporalCUnitOfWork, OfficeSharedModule } from '@officeNg';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        IntertemporalCRoutingModule
    ],
    declarations: [
        IntertemporalCComponent
    ],
    providers: [
        IntertemporalCUnitOfWork
    ]
})
export class IntertemporalCModule { }
