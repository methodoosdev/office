import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EmailAccountListComponent } from './components/email-account-list';
import { EmailAccountEditComponent } from './components/email-account-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: EmailAccountListComponent },
    { path: ':id', component: EmailAccountEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class EmailAccounRoutingModule { }
