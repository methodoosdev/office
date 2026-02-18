import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import {
    FormlyEditModule, FormListModule, OfficeSharedModule, ListingF5UnitOfWork, StickyModule
} from '@officeNg';

import { ListingF5RoutingModule } from './listingF5-routing.module';
import { ListingF5Component } from './components/listingF5';
import { ListingF5DialogComponent } from './components/listingF5-dialog';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule, StickyModule,
        ListingF5RoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        ListingF5Component,
        ListingF5DialogComponent
    ],
    providers: [
        ListingF5UnitOfWork
    ]
})
export class ListingF5Module { }
