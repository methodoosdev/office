import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ScriptToolListComponent } from "./components/tool-list";

const routes: Routes = [
    {
        path: ':id/:traderId/:config',
        component: ScriptToolListComponent
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
export class ScriptToolRoutingModule { }
