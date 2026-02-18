import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AccountingOfficeEditComponent } from './accounting-office-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: ':id', component: AccountingOfficeEditComponent, canDeactivate: [CanDeactivateGuard] },
    { path: '', redirectTo: '/office/accounting-office/1', pathMatch: 'full' },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AccountingOfficeRoutingModule { }
