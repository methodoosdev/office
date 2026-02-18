import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SimpleTaskNatureListComponent } from './components/simple-task-nature-list';
import { SimpleTaskNatureEditComponent } from './components/simple-task-nature-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: SimpleTaskNatureListComponent },
    { path: ':id', component: SimpleTaskNatureEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SimpleTaskNatureRoutingModule { }
