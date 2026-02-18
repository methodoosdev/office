import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { VatExemptionApprovalListComponent } from './components/approval-list';
import { VatExemptionApprovalEditComponent } from './components/approval-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: VatExemptionApprovalListComponent },
    { path: ':id', component: VatExemptionApprovalEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class VatExemptionApprovalRoutingModule { }
