import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderMembershipEditComponent } from './components/trader-membership-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: ':id/:parentId', component: TraderMembershipEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderMembershipRoutingModule { }
