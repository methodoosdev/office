import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerCatalogByTraderRoutingModule } from './by-trader-routing.module';
import { WorkerCatalogByTraderListComponent } from './components/by-trader-list';
import { FormListModule, FormlyEditModule, WorkerCatalogByTraderUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkerCatalogByTraderRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkerCatalogByTraderListComponent
    ],
    providers: [
        WorkerCatalogByTraderUnitOfWork
    ]
})
export class WorkerCatalogByTraderModule { }
