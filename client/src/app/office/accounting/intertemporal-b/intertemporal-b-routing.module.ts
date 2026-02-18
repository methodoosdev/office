import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { IntertemporalBComponent } from './components/intertemporal-b';

const routes: Routes = [
    { path: '', component: IntertemporalBComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class IntertemporalBRoutingModule { }
