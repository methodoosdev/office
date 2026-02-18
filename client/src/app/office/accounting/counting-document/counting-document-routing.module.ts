import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CountingDocumentComponent } from './components/counting-document';

const routes: Routes = [
    { path: '', component: CountingDocumentComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class CountingDocumentRoutingModule { }
