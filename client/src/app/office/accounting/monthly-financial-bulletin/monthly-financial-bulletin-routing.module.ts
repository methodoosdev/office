import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MonthlyFinancialBulletinComponent } from './components/monthly-financial-bulletin';

const routes: Routes = [
    { path: '', component: MonthlyFinancialBulletinComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class MonthlyFinancialBulletinRoutingModule { }
