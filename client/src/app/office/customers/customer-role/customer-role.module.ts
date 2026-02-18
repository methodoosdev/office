import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CustomerRoleRoutingModule } from './customer-role-routing.module';
import { CustomerRoleListComponent } from './components/customer-role-list';
import { CustomerRoleEditComponent } from './components/customer-role-edit';
import { FormListModule, FormlyEditModule, CustomerRoleUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        CustomerRoleRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        CustomerRoleListComponent,
        CustomerRoleEditComponent
    ],
    providers: [
        CustomerRoleUnitOfWork
    ]
})
export class CustomerRoleModule { }
