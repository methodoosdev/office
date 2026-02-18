import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ScheduleTaskListComponent } from './components/schedule-task-list';
import { ScheduleTaskEditComponent } from './components/schedule-task-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: ScheduleTaskListComponent },
    { path: ':id', component: ScheduleTaskEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ScheduleTaskRoutingModule { }
