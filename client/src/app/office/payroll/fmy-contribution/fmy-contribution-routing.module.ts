import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { FmyContributionComponent } from './components/fmy-contribution';

const routes: Routes = [
    { path: '', component: FmyContributionComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class FmyContributionRoutingModule { }
