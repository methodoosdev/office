import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ScriptDiagramComponent } from "./components/diagram";

const routes: Routes = [
    {
        path: ':id/:traderId',
        component: ScriptDiagramComponent
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
export class ScriptDiagramRoutingModule { }
