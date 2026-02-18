import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderRatingCategoryRoutingModule } from './category-routing.module';
import { TraderRatingCategoryListComponent } from './components/category-list';
import { TraderRatingCategoryEditComponent } from './components/category-edit';
import { FormlyEditModule, FormListModule, TraderRatingCategoryUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderRatingCategoryRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        TraderRatingCategoryListComponent,
        TraderRatingCategoryEditComponent
    ],
    providers: [
        TraderRatingCategoryUnitOfWork
    ]
})
export class TraderRatingCategoryModule { }
