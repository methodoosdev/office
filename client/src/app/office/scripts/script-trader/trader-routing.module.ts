import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ScriptTraderListComponent } from './components/trader-list';
import { ScriptTraderEditComponent } from './components/trader-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: ScriptTraderListComponent },
    { path: ':id', component: ScriptTraderEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ScriptTraderRoutingModule { }
