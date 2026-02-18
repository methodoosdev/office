import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EmployeeActivityLogComponent } from './components/employee-activity-log';

const routes: Routes = [
    { path: '', component: EmployeeActivityLogComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class EmployeeActivityLogRoutingModule { }
