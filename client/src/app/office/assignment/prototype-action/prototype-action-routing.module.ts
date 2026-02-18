import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AssignmentPrototypeActionListComponent } from './components/prototype-action-list';
import { AssignmentPrototypeActionEditComponent } from './components/prototype-action-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: AssignmentPrototypeActionListComponent },
    { path: ':id', component: AssignmentPrototypeActionEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AssignmentPrototypeActionRoutingModule { }
