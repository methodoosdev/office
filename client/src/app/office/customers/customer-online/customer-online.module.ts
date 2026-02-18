import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CustomerOnlineRoutingModule } from './customer-online-routing.module';
import { CustomerOnlineListComponent } from './components/customer-online-list';
import { FormListModule, FormlyEditModule, CustomerOnlineUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        CustomerOnlineRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        CustomerOnlineListComponent
    ],
    providers: [
        CustomerOnlineUnitOfWork
    ]
})
export class CustomerOnlineModule { }
