import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CanDeactivateGuard } from '@jwtNg';
import { WorkerScheduleByEmployeeEditComponent } from './components/edit-by-employee';
import { WorkerScheduleByEmployeeListComponent } from './components/list-by-employee';

const routes: Routes = [
    { path: '', component: WorkerScheduleByEmployeeListComponent },
    { path: ':id', component: WorkerScheduleByEmployeeEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerScheduleByEmployeeRoutingModule { }
