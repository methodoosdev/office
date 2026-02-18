import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { IntertemporalCComponent } from './components/intertemporal-c';

const routes: Routes = [
    { path: '', component: IntertemporalCComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class IntertemporalCRoutingModule { }
