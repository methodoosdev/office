import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PeriodicityItemListComponent } from './components/periodicity-item-list';
import { PeriodicityItemEditComponent } from './components/periodicity-item-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: PeriodicityItemListComponent },
    { path: ':id', component: PeriodicityItemEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class PeriodicityItemRoutingModule { }
