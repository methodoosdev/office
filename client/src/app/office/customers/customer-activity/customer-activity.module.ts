import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PrimeSharedModule } from '@primeNg';
import { CustomerActivityRoutingModule } from './customer-activity-routing.module';
import { CustomerActivityListComponent } from './components/customer-activity-list';
import { FormlyEditModule, FormListModule, CustomerActivityUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        PrimeSharedModule,
        OfficeSharedModule,
        CustomerActivityRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        CustomerActivityListComponent
    ],
    providers: [
        CustomerActivityUnitOfWork
    ]
})
export class CustomerActivityModule { }
