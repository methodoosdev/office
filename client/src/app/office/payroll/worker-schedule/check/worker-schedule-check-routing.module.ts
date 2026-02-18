import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerScheduleCheckComponent } from './components/worker-schedule-check';

const routes: Routes = [
    { path: ':id', component: WorkerScheduleCheckComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerScheduleCheckRoutingModule { }
