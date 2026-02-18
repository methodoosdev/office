import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LanguageListComponent } from './components/language-list';
import { LanguageEditComponent } from './components/language-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: LanguageListComponent },
    { path: ':id', component: LanguageEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class LanguageRoutingModule { }
