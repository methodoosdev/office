import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BankingTraderListComponent } from './components/trader-list';
import { BankingTraderEditComponent } from './components/trader-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: BankingTraderListComponent },
    { path: ':id', component: BankingTraderEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class BankingTraderRoutingModule { }
