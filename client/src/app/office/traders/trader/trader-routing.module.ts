import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderListComponent } from './components/trader-list';
import { TraderEditComponent } from './components/trader-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: TraderListComponent },
    { path: ':id', component: TraderEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderRoutingModule { }
