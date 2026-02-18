import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ScriptPivotListComponent } from "./components/pivot-list";

const routes: Routes = [
    {
        path: ':id/:parentId/:categoryBookTypeId/:year/:period/:showTypeId/:inventory',
        component: ScriptPivotListComponent
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ScriptPivotRoutingModule { }
