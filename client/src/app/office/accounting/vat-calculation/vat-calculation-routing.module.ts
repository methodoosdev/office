import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { VatCalculationComponent } from './components/vat-calculation';

const routes: Routes = [
    { path: '', component: VatCalculationComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class VatCalculationRoutingModule { }
