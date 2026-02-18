import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AccountingWorkListComponent } from './components/accounting-work-list';
import { AccountingWorkEditComponent } from './components/accounting-work-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: AccountingWorkListComponent },
    { path: ':id', component: AccountingWorkEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AccountingWorkRoutingModule { }
