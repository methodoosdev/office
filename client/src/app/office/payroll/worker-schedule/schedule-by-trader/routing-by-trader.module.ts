import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CanDeactivateGuard } from '@jwtNg';
import { WorkerScheduleByTraderEditComponent } from './components/edit-by-trader';
import { WorkerScheduleByTraderListComponent } from './components/list-by-trader';

const routes: Routes = [
    { path: '', component: WorkerScheduleByTraderListComponent },
    { path: ':id', component: WorkerScheduleByTraderEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerScheduleByTraderRoutingModule { }
