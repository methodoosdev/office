import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CustomerPermissionRoutingModule } from './customer-permission-routing.module';
import { CustomerPermissionListComponent } from './components/customer-permission-list';
import { CustomerPermissionEditComponent } from './components/customer-permission-edit';
import { FormListModule, FormlyEditModule, CustomerPermissionUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        CustomerPermissionRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        CustomerPermissionListComponent,
        CustomerPermissionEditComponent
    ],
    providers: [
        CustomerPermissionUnitOfWork
    ]
})
export class CustomerPermissionModule { }
