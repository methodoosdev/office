import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ChamberRoutingModule } from './chamber-routing.module';
import { ChamberListComponent } from './components/chamber-list';
import { ChamberEditComponent } from './components/chamber-edit';
import { FormListModule, FormlyEditModule, ChamberUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ChamberRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        ChamberListComponent,
        ChamberEditComponent
    ],
    providers: [
        ChamberUnitOfWork
    ]
})
export class ChamberModule { }
