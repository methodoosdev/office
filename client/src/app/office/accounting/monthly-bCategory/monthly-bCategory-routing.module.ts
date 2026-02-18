import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MonthlyBCategoryBulletinComponent } from './components/monthly-bCategory';

const routes: Routes = [
    { path: '', component: MonthlyBCategoryBulletinComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class MonthlyBCategoryBulletinRoutingModule { }
