import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { NbgListComponent } from './components/nbg-list';

const routes: Routes = [
    { path: '', component: NbgListComponent },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class NbgRoutingModule { }
