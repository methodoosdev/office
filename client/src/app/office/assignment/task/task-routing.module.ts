import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AssignmentTaskListComponent } from './components/task-list';
import { AssignmentTaskEditComponent } from './components/task-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: AssignmentTaskListComponent },
    { path: ':id', component: AssignmentTaskEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AssignmentTaskRoutingModule { }
