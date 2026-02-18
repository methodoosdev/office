import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ToastrModule } from "ngx-toastr";

import { GridModule, ExcelModule, PDFModule } from "@progress/kendo-angular-grid";
import { IntlModule } from "@progress/kendo-angular-intl";
import { ButtonModule, ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogModule } from "@progress/kendo-angular-dialog";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { MenusModule } from "@progress/kendo-angular-menu";

import { FormListComponent } from "./form-list.component";
import { OfficeSharedModule } from "../../shared/office-shared.module";
import { MoreLessModule } from "../../components/more-less/more-less";
import { PrimeSharedModule } from "@primeNg";
import { StickyModule } from "../../api/sticky";

@NgModule({
    imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule,
        PrimeSharedModule, OfficeSharedModule, StickyModule,
        GridModule, ExcelModule, PDFModule, ButtonModule, ButtonsModule, MenusModule,
        InputsModule, DialogModule, ToastrModule, IntlModule, MoreLessModule],
    exports: [FormListComponent],
    declarations: [FormListComponent]
})
export class FormListModule { }
