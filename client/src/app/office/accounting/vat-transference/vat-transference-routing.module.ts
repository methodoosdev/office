import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { VatTransferenceComponent } from './components/vat-transference';

const routes: Routes = [
    { path: '', component: VatTransferenceComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class VatTransferenceRoutingModule { }
