import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderActivityLogComponent } from './components/trader-activity-log';

const routes: Routes = [
    { path: '', component: TraderActivityLogComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderActivityLogRoutingModule { }
