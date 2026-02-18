import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ListingF4Component } from './components/listingF4';

const routes: Routes = [
    { path: '', component: ListingF4Component }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ListingF4RoutingModule { }
