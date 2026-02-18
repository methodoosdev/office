import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ScriptTableNameListComponent } from './components/table-name-list';
import { ScriptTableNameEditComponent } from './components/table-name-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: ScriptTableNameListComponent },
    { path: ':id', component: ScriptTableNameEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ScriptTableNameRoutingModule { }
