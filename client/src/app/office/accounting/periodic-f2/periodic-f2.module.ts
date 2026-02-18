import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PDFExportModule } from '@progress/kendo-angular-pdf-export';

import { FormlyKendoModule } from '@formlyNg';
import { PrimeSharedModule } from '@primeNg';
import { FormListModule, FormEditModule, OfficeSharedModule, PeriodicF2UnitOfWork, StickyModule } from '@officeNg';

import { PeriodicF2RoutingModule } from './periodic-f2-routing.module';
import { PeriodicF2ListComponent } from './components/periodic-f2-list';
import { PeriodicF2EditComponent } from './components/periodic-f2-edit';
import { PeriodicF2DialogComponent } from './components/periodic-f2-dialog';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        StickyModule,
        PrimeSharedModule,
        PDFExportModule,
        PeriodicF2RoutingModule,
        FormListModule,
        FormEditModule,
        FormlyKendoModule
    ],
    declarations: [
        PeriodicF2DialogComponent,
        PeriodicF2ListComponent,
        PeriodicF2EditComponent
    ],
    providers: [
        PeriodicF2UnitOfWork
    ]
})
export class PeriodicF2Module { }
