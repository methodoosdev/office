import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AssignmentPrototypeListComponent } from './components/prototype-list';
import { AssignmentPrototypeEditComponent } from './components/prototype-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: AssignmentPrototypeListComponent },
    { path: ':id', component: AssignmentPrototypeEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AssignmentPrototypeRoutingModule { }
