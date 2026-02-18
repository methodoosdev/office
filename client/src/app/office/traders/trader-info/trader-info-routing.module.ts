import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderInfoEditComponent } from './components/trader-info-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: ':id/:parentId', component: TraderInfoEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderInfoRoutingModule { }
