import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PeriodicityItemsRoutingModule } from './periodicity-items-routing.module';
import { PeriodicityItemsComponent } from './components/periodicity-items';
import { FormListModule, FormlyEditModule, OfficeSharedModule, StickyModule, PeriodicityItemsUnitOfWork } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule, StickyModule,
        FormListModule,
        FormlyEditModule,
        PeriodicityItemsRoutingModule
    ],
    declarations: [
        PeriodicityItemsComponent
    ],
    providers: [
        PeriodicityItemsUnitOfWork
    ]
})
export class PeriodicityItemsModule { }
