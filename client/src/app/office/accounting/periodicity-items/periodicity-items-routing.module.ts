import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PeriodicityItemsComponent } from './components/periodicity-items';

const routes: Routes = [
    { path: '', component: PeriodicityItemsComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class PeriodicityItemsRoutingModule { }
