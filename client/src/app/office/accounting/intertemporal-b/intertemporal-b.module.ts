import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IntertemporalBRoutingModule } from './intertemporal-b-routing.module';
import { IntertemporalBComponent } from './components/intertemporal-b';
import { FormListModule, FormlyEditModule, IntertemporalBUnitOfWork, OfficeSharedModule } from '@officeNg';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        IntertemporalBRoutingModule
    ],
    declarations: [
        IntertemporalBComponent
    ],
    providers: [
        IntertemporalBUnitOfWork
    ]
})
export class IntertemporalBModule { }
