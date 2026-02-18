import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SimpleTaskDepartmentListComponent } from './components/simple-task-department-list';
import { SimpleTaskDepartmentEditComponent } from './components/simple-task-department-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: SimpleTaskDepartmentListComponent },
    { path: ':id', component: SimpleTaskDepartmentEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SimpleTaskDepartmentRoutingModule { }
