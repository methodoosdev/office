import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerListComponent } from './components/customer-list';
import { CustomerEditComponent } from './components/customer-edit';
import { AuthGuard, CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        children: [
            { path: '', component: CustomerListComponent },
            { path: ':id', component: CustomerEditComponent, canDeactivate: [CanDeactivateGuard] }
        ]
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
export class CustomerRoutingModule { }
