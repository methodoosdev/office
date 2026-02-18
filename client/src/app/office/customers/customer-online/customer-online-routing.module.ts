import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerOnlineListComponent } from './components/customer-online-list';

const routes: Routes = [
    { path: '', component: CustomerOnlineListComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CustomerOnlineRoutingModule { }
