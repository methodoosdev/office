import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ChamberListComponent } from './components/chamber-list';
import { ChamberEditComponent } from './components/chamber-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: ChamberListComponent },
    { path: ':id', component: ChamberEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ChamberRoutingModule { }
