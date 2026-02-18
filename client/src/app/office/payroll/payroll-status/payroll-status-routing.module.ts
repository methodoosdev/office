import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PayrollStatusComponent } from './components/payroll-status';

const routes: Routes = [
    { path: '', component: PayrollStatusComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class PayrollStatusRoutingModule { }
