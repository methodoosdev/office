import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerLeaveDetailListComponent } from './components/leave-detail-list';
import { WorkerLeaveDetailEditComponent } from './components/leave-detail-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: WorkerLeaveDetailListComponent },
    { path: ':id', component: WorkerLeaveDetailEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerLeaveDetailRoutingModule { }
