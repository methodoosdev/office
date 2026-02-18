import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SecurityComponent } from './components/security.component';

const routes: Routes = [
    { path: '', component: SecurityComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SecurityRoutingModule { }
