import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SoftoneProjectRoutingModule } from './softone-project-routing.module';
import { SoftoneProjectComponent } from './components/softone-project';
import { SoftoneProjectDetailComponent } from './components/softone-project-detail';
import { PrimeSharedModule } from '@primeNg';
import {
    FormListDetailModule, FormListModule, FormlyEditModule, SoftoneProjectUnitOfWork,
    SoftoneProjectDetailUnitOfWork, OfficeSharedModule, StickyModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PrimeSharedModule,
        FormlyEditModule,
        FormListModule,
        FormListDetailModule,
        SoftoneProjectRoutingModule,
        StickyModule
    ],
    declarations: [
        SoftoneProjectComponent,
        SoftoneProjectDetailComponent
    ],
    providers: [
        SoftoneProjectUnitOfWork,
        SoftoneProjectDetailUnitOfWork
    ]
})
export class SoftoneProjectModule { }
