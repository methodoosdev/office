import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ApdTekaRoutingModule } from './apd-teka-routing.module';
import { ApdTekaListComponent } from './components/apd-teka-list';
import { ApdTekaEditComponent } from './components/apd-teka-edit';
import {
    FormlyEditModule, FormListModule, FormListDialogModule, ApdTekaUnitOfWork,
    OfficeSharedModule, TraderLookupUnitOfWork, FormlyEditDialogModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ApdTekaRoutingModule,
        FormlyEditDialogModule,
        FormListModule,
        FormlyEditModule,
        FormListDialogModule
    ],
    declarations: [
        ApdTekaListComponent,
        ApdTekaEditComponent
    ],
    providers: [
        ApdTekaUnitOfWork,
        TraderLookupUnitOfWork
    ]
})
export class ApdTekaModule { }
