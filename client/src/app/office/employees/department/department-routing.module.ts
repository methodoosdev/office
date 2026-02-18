import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DepartmentListComponent } from './components/department-list';
import { DepartmentEditComponent } from './components/department-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: DepartmentListComponent },
    { path: ':id', component: DepartmentEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class DepartmentRoutingModule { }
