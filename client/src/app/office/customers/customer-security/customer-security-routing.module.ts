import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerSecurityComponent } from './components/customer-security.component';

const routes: Routes = [
    { path: '', component: CustomerSecurityComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CustomerSecurityRoutingModule { }
