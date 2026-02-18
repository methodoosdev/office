import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CanDeactivateGuard } from '@jwtNg';
import { WorkerScheduleSubmitComponent } from './components/worker-schedule-submit';

const routes: Routes = [
    { path: ':id', component: WorkerScheduleSubmitComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerScheduleSubmitRoutingModule { }
