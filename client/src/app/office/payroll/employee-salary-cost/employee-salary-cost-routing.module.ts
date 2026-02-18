import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EmployeeSalaryCostComponent } from './components/employee-salary-cost';

const routes: Routes = [
    { path: '', component: EmployeeSalaryCostComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class EmployeeSalaryCostRoutingModule { }
