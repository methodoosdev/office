import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderGroupRoutingModule } from './trader-group-routing.module';
import { TraderGroupListComponent } from './components/trader-group-list';
import { TraderGroupEditComponent } from './components/trader-group-edit';
import { FormListModule, FormlyEditModule, TraderGroupUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderGroupRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        TraderGroupListComponent,
        TraderGroupEditComponent
    ],
    providers: [
        TraderGroupUnitOfWork
    ]
})
export class TraderGroupModule { }
