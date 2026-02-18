import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderMonthlyBillingEditComponent } from './components/trader-monthly-billing-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: ':id/:parentId', component: TraderMonthlyBillingEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderMonthlyBillingRoutingModule { }
