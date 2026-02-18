import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderRelationshipRoutingModule } from './trader-relationship-routing.module';
import { TraderRelationshipEditComponent } from './components/trader-relationship-edit';
import { FormListModule, FormlyEditNewModule, TraderRelationshipUnitOfWork, OfficeSharedModule } from '@officeNg';
//bugathina
@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderRelationshipRoutingModule,
        FormListModule,
        FormlyEditNewModule
    ],
    declarations: [
        TraderRelationshipEditComponent
    ],
    providers: [
        TraderRelationshipUnitOfWork
    ]
})
export class TraderRelationshipModule { }
