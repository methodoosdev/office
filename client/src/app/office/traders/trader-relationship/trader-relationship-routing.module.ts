import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderRelationshipEditComponent } from './components/trader-relationship-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: ':id/:parentId', component: TraderRelationshipEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderRelationshipRoutingModule { }
