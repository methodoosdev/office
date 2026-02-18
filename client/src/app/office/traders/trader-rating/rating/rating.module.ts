import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderRatingRoutingModule } from './rating-routing.module';
import { TraderRatingListComponent } from './components/rating-list';
import { TraderRatingEditComponent } from './components/rating-edit';
import { FormlyEditModule, FormListModule, TraderRatingUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderRatingRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        TraderRatingListComponent,
        TraderRatingEditComponent
    ],
    providers: [
        TraderRatingUnitOfWork
    ]
})
export class TraderRatingModule { }
