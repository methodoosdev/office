import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { GridModule } from "@progress/kendo-angular-grid";
import { IntlModule } from "@progress/kendo-angular-intl";
import { ButtonModule, ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogModule } from "@progress/kendo-angular-dialog";
import { InputsModule } from "@progress/kendo-angular-inputs";

import { FormListDialogComponent } from "./form-list-dialog.component";
import { OfficeSharedModule } from "../../shared/office-shared.module";

@NgModule({
    imports: [CommonModule, RouterModule, OfficeSharedModule, GridModule, ButtonModule, ButtonsModule, InputsModule, DialogModule, IntlModule],
    exports: [FormListDialogComponent],
    declarations: [FormListDialogComponent]
})
export class FormListDialogModule { }
