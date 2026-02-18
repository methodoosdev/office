import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PeriodicityItemRoutingModule } from './periodicity-item-routing.module';
import { PeriodicityItemListComponent } from './components/periodicity-item-list';
import { PeriodicityItemEditComponent } from './components/periodicity-item-edit';
import { FormListModule, FormlyEditModule, PeriodicityItemUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PeriodicityItemRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        PeriodicityItemListComponent,
        PeriodicityItemEditComponent
    ],
    providers: [
        PeriodicityItemUnitOfWork
    ]
})
export class PeriodicityItemModule { }
