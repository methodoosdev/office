import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkingAreaListComponent } from './components/working-area-list';
import { WorkingAreaEditComponent } from './components/working-area-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: WorkingAreaListComponent },
    { path: ':id', component: WorkingAreaEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkingAreaRoutingModule { }
