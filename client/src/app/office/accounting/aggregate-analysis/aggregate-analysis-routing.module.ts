import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AggregateAnalysisComponent } from './components/aggregate-analysis';

const routes: Routes = [
    { path: '', component: AggregateAnalysisComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AggregateAnalysisRoutingModule { }
