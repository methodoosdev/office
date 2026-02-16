import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ToastrModule } from "ngx-toastr";

import { GridModule } from "@progress/kendo-angular-grid";
import { IntlModule } from "@progress/kendo-angular-intl";
import { ButtonModule, ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogModule } from "@progress/kendo-angular-dialog";
import { InputsModule } from "@progress/kendo-angular-inputs";

import { FormListDetailMappingComponent } from "./form-list-detail-mapping.component";
import { FormListDialogComponent } from "../form-list-dialog/form-list-dialog.component";
import { OfficeSharedModule } from "../../shared/office-shared.module";

@NgModule({
    imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule,
        OfficeSharedModule, GridModule, ButtonModule, ButtonsModule, InputsModule, DialogModule, ToastrModule, IntlModule],
    exports: [FormListDetailMappingComponent],
    declarations: [FormListDetailMappingComponent],
    //entryComponents: [FormListDialogComponent]
})
export class FormListDetailMappingModule { }
