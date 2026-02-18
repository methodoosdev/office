import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EducationListComponent } from './components/education-list';
import { EducationEditComponent } from './components/education-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: EducationListComponent },
    { path: ':id', component: EducationEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class EducationRoutingModule { }
