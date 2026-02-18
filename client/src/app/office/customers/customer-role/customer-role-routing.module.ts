import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerRoleListComponent } from './components/customer-role-list';
import { CustomerRoleEditComponent } from './components/customer-role-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: CustomerRoleListComponent },
    { path: ':id', component: CustomerRoleEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CustomerRoleRoutingModule { }
