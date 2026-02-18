import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { QueuedEmailListComponent } from './components/queued-email-list';
import { QueuedEmailEditComponent } from './components/queued-email-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: QueuedEmailListComponent },
    { path: ':id', component: QueuedEmailEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class QueuedEmailRoutingModule { }
