import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AssignmentReasonListComponent } from './components/reason-list';
import { AssignmentReasonEditComponent } from './components/reason-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: AssignmentReasonListComponent },
    { path: ':id', component: AssignmentReasonEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AssignmentReasonRoutingModule { }
