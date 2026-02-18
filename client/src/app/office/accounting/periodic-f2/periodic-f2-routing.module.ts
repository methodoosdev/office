import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PeriodicF2ListComponent } from './components/periodic-f2-list';
import { PeriodicF2EditComponent } from './components/periodic-f2-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: PeriodicF2ListComponent },
    { path: ':id', component: PeriodicF2EditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class PeriodicF2RoutingModule { }
