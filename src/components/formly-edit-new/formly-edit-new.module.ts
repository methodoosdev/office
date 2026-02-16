import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { FormlyModule } from "@ngx-formly/core";
import { ToastrModule } from "ngx-toastr";

import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogModule } from "@progress/kendo-angular-dialog";
import { InputsModule } from "@progress/kendo-angular-inputs";

import { FormlyEditNewComponent } from "./formly-edit-new.component";
import { FormlyKendoModule } from "@formlyNg";
import { PrimeSharedModule } from "@primeNg";
import { StickyModule } from "../../api/sticky";


@NgModule({
    imports: [
        CommonModule, FormsModule, ReactiveFormsModule, RouterModule, StickyModule,
        ButtonsModule, InputsModule, DialogModule, PrimeSharedModule,
        ToastrModule, FormlyModule, FormlyKendoModule
    ],
    exports: [
        FormlyEditNewComponent
    ],
    declarations: [
        FormlyEditNewComponent
    ]
})
export class FormlyEditNewModule { }
