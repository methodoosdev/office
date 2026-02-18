import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PayrollCheckComponent } from './components/payroll-check';

const routes: Routes = [
    { path: '', component: PayrollCheckComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class PayrollCheckRoutingModule { }
