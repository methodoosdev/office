import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ApdContributionComponent } from './components/apd-contribution';

const routes: Routes = [
    { path: '', component: ApdContributionComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ApdContributionRoutingModule { }
