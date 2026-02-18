import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ListingF5Component } from './components/listingF5';

const routes: Routes = [
    { path: '', component: ListingF5Component }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ListingF5RoutingModule { }
