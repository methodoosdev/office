import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { FmySubmissionComponent } from './components/fmy-submission';

const routes: Routes = [
    { path: '', component: FmySubmissionComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class FmySubmissionRoutingModule { }
