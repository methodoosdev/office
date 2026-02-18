import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { JobTitleListComponent } from './components/jobTitle-list';
import { JobTitleEditComponent } from './components/jobTitle-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: JobTitleListComponent },
    { path: ':id', component: JobTitleEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class JobTitleRoutingModule { }
