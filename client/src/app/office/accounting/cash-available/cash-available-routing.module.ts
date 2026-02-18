import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CashAvailableComponent } from './components/cash-available';

const routes: Routes = [
    { path: '', component: CashAvailableComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CashAvailableRoutingModule { }
