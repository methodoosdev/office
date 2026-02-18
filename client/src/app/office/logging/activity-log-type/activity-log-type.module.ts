import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivityLogTypeRoutingModule } from './activity-log-type-routing.module';
import { ActivityLogTypeListComponent } from './components/activity-log-type-list';
import { ActivityLogTypeEditComponent } from './components/activity-log-type-edit';
import { FormlyEditModule, FormListModule, ActivityLogTypeUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ActivityLogTypeRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        ActivityLogTypeListComponent,
        ActivityLogTypeEditComponent
    ],
    providers: [
        ActivityLogTypeUnitOfWork
    ]
})
export class ActivityLogTypeModule { }
