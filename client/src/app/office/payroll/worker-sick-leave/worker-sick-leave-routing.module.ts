import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerSickLeaveComponent } from './components/worker-sick-leave';

const routes: Routes = [
    { path: '', component: WorkerSickLeaveComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerSickLeaveRoutingModule { }