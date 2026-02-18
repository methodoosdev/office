import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { VatExemptionDocListComponent } from './components/doc-list';
import { VatExemptionDocEditComponent } from './components/doc-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: VatExemptionDocListComponent },
    { path: ':id', component: VatExemptionDocEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class VatExemptionDocRoutingModule { }
