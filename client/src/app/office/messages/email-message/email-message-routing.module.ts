import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EmailMessageListComponent } from './components/email-message-list';
import { EmailMessageEditComponent } from './components/email-message-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: EmailMessageListComponent },
    { path: ':id', component: EmailMessageEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class EmailMessageRoutingModule { }
