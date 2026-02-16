import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { FormlyModule } from "@ngx-formly/core";

import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogModule } from "@progress/kendo-angular-dialog";

import { FormlyEditDialogComponent } from "./formly-edit-dialog.component";
import { FormlyKendoModule } from "@formlyNg";


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        ButtonsModule,
        DialogModule,
        FormlyModule,
        FormlyKendoModule
    ],
    exports: [
        FormlyEditDialogComponent
    ],
    declarations: [
        FormlyEditDialogComponent
    ]
})
export class FormlyEditDialogModule { }
