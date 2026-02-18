import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderGroupListComponent } from './components/trader-group-list';
import { TraderGroupEditComponent } from './components/trader-group-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: TraderGroupListComponent },
    { path: ':id', component: TraderGroupEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderGroupRoutingModule { }
