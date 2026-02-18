import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoadExcelComponent } from './components/load-excel';

const routes: Routes = [
    { path: '', component: LoadExcelComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class LoadExcelRoutingModule { }
