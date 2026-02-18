import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PayoffLiabilitiesComponent } from './components/payoff-liabilities';

const routes: Routes = [
    { path: '', component: PayoffLiabilitiesComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class PayoffLiabilitiesRoutingModule { }
