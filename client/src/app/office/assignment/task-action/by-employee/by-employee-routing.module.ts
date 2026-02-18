import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AssignmentTaskActionByEmployeeListComponent } from './components/by-employee-list';
import { AssignmentTaskActionByEmployeeEditComponent } from './components/by-employee-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: AssignmentTaskActionByEmployeeListComponent },
    { path: ':id', component: AssignmentTaskActionByEmployeeEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AssignmentTaskActionByEmployeeRoutingModule { }
