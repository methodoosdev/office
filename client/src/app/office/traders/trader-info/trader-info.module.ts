import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderInfoRoutingModule } from './trader-info-routing.module';
import { TraderInfoEditComponent } from './components/trader-info-edit';
import { FormListModule, FormlyEditNewModule, TraderInfoUnitOfWork, OfficeSharedModule } from '@officeNg';
//bugathina
@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderInfoRoutingModule,
        FormListModule,
        FormlyEditNewModule
    ],
    declarations: [
        TraderInfoEditComponent
    ],
    providers: [
        TraderInfoUnitOfWork
    ]
})
export class TraderInfoModule { }
