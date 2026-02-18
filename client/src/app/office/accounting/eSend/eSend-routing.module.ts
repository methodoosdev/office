import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ESendComponent } from './components/eSend';

const routes: Routes = [
    { path: '', component: ESendComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ESendRoutingModule { }
