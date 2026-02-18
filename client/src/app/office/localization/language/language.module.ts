import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LanguageRoutingModule } from './language-routing.module';
import { LanguageListComponent } from './components/language-list';
import { LanguageEditComponent } from './components/language-edit';
import { FormListModule, FormlyEditModule, LanguageUnitOfWork, OfficeSharedModule } from '@officeNg';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        LanguageRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        LanguageListComponent,
        LanguageEditComponent
    ],
    providers: [
        LanguageUnitOfWork
    ]
})
export class LanguageModule { }
