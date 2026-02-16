import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ToastrModule } from "ngx-toastr";

import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogModule } from "@progress/kendo-angular-dialog";
import { InputsModule } from "@progress/kendo-angular-inputs";

import { FormEditComponent } from "./form-edit.component";
import { PrimeSharedModule } from "@primeNg";


@NgModule({
    imports: [
        CommonModule, FormsModule, ReactiveFormsModule, RouterModule,
        ButtonsModule, InputsModule, DialogModule, ToastrModule, PrimeSharedModule
    ],
    exports: [
        FormEditComponent
    ],
    declarations: [
        FormEditComponent
    ]
})
export class FormEditModule { }
