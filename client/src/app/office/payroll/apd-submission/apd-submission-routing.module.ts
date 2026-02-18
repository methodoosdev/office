import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ApdSubmissionComponent } from './components/apd-submission';

const routes: Routes = [
    { path: '', component: ApdSubmissionComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ApdSubmissionRoutingModule { }
