import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { VatExemptionSerialListComponent } from './components/serial-list';
import { VatExemptionSerialEditComponent } from './components/serial-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: VatExemptionSerialListComponent },
    { path: ':id', component: VatExemptionSerialEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class VatExemptionSerialRoutingModule { }
