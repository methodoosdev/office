import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MyDataItemListComponent } from './components/myData-item-list';
import { MyDataItemEditComponent } from './components/myData-item-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: MyDataItemListComponent },
    { path: ':id', component: MyDataItemEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class MyDataItemRoutingModule { }
