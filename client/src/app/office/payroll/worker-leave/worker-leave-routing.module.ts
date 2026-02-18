import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerLeaveComponent } from './components/worker-leave';

const routes: Routes = [
    { path: '', component: WorkerLeaveComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerLeaveRoutingModule { }