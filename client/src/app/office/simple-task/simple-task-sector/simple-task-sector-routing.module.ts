import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SimpleTaskSectorListComponent } from './components/simple-task-sector-list';
import { SimpleTaskSectorEditComponent } from './components/simple-task-sector-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: SimpleTaskSectorListComponent },
    { path: ':id', component: SimpleTaskSectorEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SimpleTaskSectorRoutingModule { }
