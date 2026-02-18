import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { FinancialObligationListComponent } from './components/financial-obligation-list';
import { FinancialObligationEditComponent } from './components/financial-obligation-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: FinancialObligationListComponent },
    { path: ':id', component: FinancialObligationEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class FinancialObligationRoutingModule { }
