import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CustomerRoutingModule } from './customer-routing.module';
import { CustomerListComponent } from './components/customer-list';
import { CustomerEditComponent } from './components/customer-edit';
import { PrimeSharedModule } from '@primeNg';
import {
    FormlyEditModule, FormListModule, FormListDetailMappingModule, FormListDialogModule, CustomerUnitOfWork,
    CustomerPermissionUnitOfWork, CustomerPermissionsByCustomerUnitOfWork, OfficeSharedModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PrimeSharedModule,
        CustomerRoutingModule,
        FormListModule,
        FormlyEditModule,
        FormListDetailMappingModule,
        FormListDialogModule
    ],
    declarations: [
        CustomerListComponent,
        CustomerEditComponent
    ],
    providers: [
        CustomerUnitOfWork,
        CustomerPermissionUnitOfWork,
        CustomerPermissionsByCustomerUnitOfWork
    ]
})
export class CustomerModule { }
