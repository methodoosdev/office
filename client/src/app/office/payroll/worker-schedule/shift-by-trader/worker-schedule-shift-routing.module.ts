import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerScheduleShiftListComponent } from './components/worker-schedule-shift-list';
import { WorkerScheduleShiftEditComponent } from './components/worker-schedule-shift-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: WorkerScheduleShiftListComponent },
    { path: ':id', component: WorkerScheduleShiftEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerScheduleShiftRoutingModule { }
