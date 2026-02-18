import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import {
    FormListModule, FormlyEditModule, ListingF4UnitOfWork, OfficeSharedModule, StickyModule
} from '@officeNg';

import { ListingF4RoutingModule } from './listingF4-routing.module';
import { ListingF4Component } from './components/listingF4';
import { ListingF4DialogComponent } from './components/listingF4-dialog';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule, StickyModule,
        ListingF4RoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        ListingF4Component,
        ListingF4DialogComponent
    ],
    providers: [
        ListingF4UnitOfWork
    ]
})
export class ListingF4Module { }
