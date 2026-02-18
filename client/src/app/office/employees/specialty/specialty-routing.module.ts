import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SpecialtyListComponent } from './components/specialty-list';
import { SpecialtyEditComponent } from './components/specialty-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: SpecialtyListComponent },
    { path: ':id', component: SpecialtyEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SpecialtyRoutingModule { }
