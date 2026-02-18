import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SoftoneProjectComponent } from './components/softone-project';
import { SoftoneProjectDetailComponent } from './components/softone-project-detail';
import { AuthGuard } from '@jwtNg';

const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        children: [
            { path: '', component: SoftoneProjectComponent },
            { path: ':projectId/:traderId', component: SoftoneProjectDetailComponent }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SoftoneProjectRoutingModule { }
