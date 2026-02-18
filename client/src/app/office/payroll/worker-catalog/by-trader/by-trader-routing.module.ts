import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WorkerCatalogByTraderListComponent } from './components/by-trader-list';

const routes: Routes = [
    { path: '', component: WorkerCatalogByTraderListComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WorkerCatalogByTraderRoutingModule { }
