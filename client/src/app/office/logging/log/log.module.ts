import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LogRoutingModule } from './log-routing.module';
import { LogListComponent } from './components/log-list';
import { LogEditComponent } from './components/log-edit';
import {
    FormListModule, FormlyEditModule, LogUnitOfWork, PersistStateUnitOfWork, OfficeSharedModule,
    ActivityLogTypeUnitOfWork
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        LogRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        LogListComponent,
        LogEditComponent
    ],
    providers: [
        LogUnitOfWork,
        ActivityLogTypeUnitOfWork,
        PersistStateUnitOfWork
    ]
})
export class LogModule { }
