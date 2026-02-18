import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { BookmarkRoutingModule } from './bookmark-routing.module';
import { BookmarkListComponent } from './components/bookmark-list';
import { BookmarkEditComponent } from './components/bookmark-edit';
import { FormlyEditModule, FormListModule, BookmarkUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        BookmarkRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        BookmarkListComponent,
        BookmarkEditComponent
    ],
    providers: [
        BookmarkUnitOfWork
    ]
})
export class BookmarkModule { }
