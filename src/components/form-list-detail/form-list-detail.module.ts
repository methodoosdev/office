import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ToastrModule } from "ngx-toastr";

import { GridModule, ExcelModule, PDFModule } from "@progress/kendo-angular-grid";
import { IntlModule } from "@progress/kendo-angular-intl";
import { ButtonModule, ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogModule } from "@progress/kendo-angular-dialog";
import { InputsModule } from "@progress/kendo-angular-inputs";

import { FormListDetailComponent } from "./form-list-detail.component";
import { OfficeSharedModule } from "../../shared/office-shared.module";

@NgModule({
    imports: [CommonModule, RouterModule, OfficeSharedModule, GridModule, ExcelModule, PDFModule,
        ButtonModule, ButtonsModule, InputsModule, DialogModule, ToastrModule, IntlModule],
    exports: [FormListDetailComponent],
    declarations: [FormListDetailComponent]
})
export class FormListDetailModule { }
