import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LogListComponent } from './components/log-list';
import { LogEditComponent } from './components/log-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: LogListComponent },
    { path: ':id', component: LogEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class LogRoutingModule { }
