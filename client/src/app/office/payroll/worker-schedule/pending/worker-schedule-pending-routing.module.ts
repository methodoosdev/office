import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerSchedulePendingListComponent } from './components/worker-schedule-pending-list';

const routes: Routes = [
    { path: '', component: WorkerSchedulePendingListComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerSchedulePendingRoutingModule { }
