import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { VatExemptionReportListComponent } from './components/report-list';
import { VatExemptionReportEditComponent } from './components/report-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: VatExemptionReportListComponent },
    { path: ':id', component: VatExemptionReportEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class VatExemptionReportRoutingModule { }
