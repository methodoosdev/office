import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerActivityLogComponent } from './components/customer-activity-log';

const routes: Routes = [
    {
        path: '',
        component: CustomerActivityLogComponent
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CustomerActivityLogRoutingModule { }
