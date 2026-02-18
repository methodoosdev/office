import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SimpleTaskManagerListComponent } from './components/simple-task-manager-list';
import { SimpleTaskManagerEditComponent } from './components/simple-task-manager-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: SimpleTaskManagerListComponent },
    { path: ':id', component: SimpleTaskManagerEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SimpleTaskManagerRoutingModule { }
