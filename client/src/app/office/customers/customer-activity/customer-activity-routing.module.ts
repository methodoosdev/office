import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerActivityListComponent } from './components/customer-activity-list';

const routes: Routes = [
    { path: '', component: CustomerActivityListComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CustomerActivityRoutingModule { }
