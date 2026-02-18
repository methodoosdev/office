import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderChargeComponent } from './components/trader-charge';

const routes: Routes = [
    { path: '', component: TraderChargeComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderChargeRoutingModule { }
