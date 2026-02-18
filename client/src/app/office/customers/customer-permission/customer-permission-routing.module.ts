import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerPermissionListComponent } from './components/customer-permission-list';
import { CustomerPermissionEditComponent } from './components/customer-permission-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: CustomerPermissionListComponent },
    { path: ':id', component: CustomerPermissionEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CustomerPermissionRoutingModule { }
