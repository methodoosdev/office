import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ApdTekaListComponent } from './components/apd-teka-list';
import { ApdTekaEditComponent } from './components/apd-teka-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: ApdTekaListComponent },
    { path: ':id', component: ApdTekaEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ApdTekaRoutingModule { }
