import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TaxFactorListComponent } from './components/tax-factor-list';
import { TaxFactorEditComponent } from './components/tax-factor-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: TaxFactorListComponent },
    { path: ':id', component: TaxFactorEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TaxFactorRoutingModule { }
