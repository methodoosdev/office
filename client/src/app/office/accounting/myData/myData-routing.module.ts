import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MyDataComponent } from './components/myData';

const routes: Routes = [
    { path: '', component: MyDataComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class MyDataRoutingModule { }
