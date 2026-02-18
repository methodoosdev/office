import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { CustomerActivityLogRoutingModule } from './customer-activity-log-routing.module';
import { CustomerActivityLogComponent } from './components/customer-activity-log';

import { CustomerActivityUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        OfficeSharedModule,
        CustomerActivityLogRoutingModule
    ],
    declarations: [
        CustomerActivityLogComponent
    ],
    providers: [
        CustomerActivityUnitOfWork
    ]
})
export class CustomerActivityLogModule { }
