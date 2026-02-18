import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CustomerSecurityUnitOfWork, OfficeSharedModule } from '@officeNg';
import { CustomerSecurityRoutingModule } from './customer-security-routing.module';
import { CustomerSecurityComponent } from './components/customer-security.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        CustomerSecurityRoutingModule
    ],
    declarations: [
        CustomerSecurityComponent
    ],
    providers: [
        CustomerSecurityUnitOfWork
    ]
})
export class CustomerSecurityModule { }
