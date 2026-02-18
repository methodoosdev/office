import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ScriptReportListComponent } from './components/report-list';

const routes: Routes = [
    { path: ':id/:categoryBookTypeId/:config', component: ScriptReportListComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ScriptReportRoutingModule { }
