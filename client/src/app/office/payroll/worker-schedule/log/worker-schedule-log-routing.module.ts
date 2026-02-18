import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerScheduleLogListComponent } from './components/worker-schedule-log-list';
import { WorkerScheduleLogEditComponent } from './components/worker-schedule-log-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: WorkerScheduleLogListComponent },
    { path: ':id', component: WorkerScheduleLogEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerScheduleLogRoutingModule { }
