import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ActivityLogTypeListComponent } from './components/activity-log-type-list';
import { ActivityLogTypeEditComponent } from './components/activity-log-type-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: ActivityLogTypeListComponent },
    { path: ':id', component: ActivityLogTypeEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ActivityLogTypeRoutingModule { }
