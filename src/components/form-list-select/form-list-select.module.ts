import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { FormsModule } from "@angular/forms";

import { GridModule } from "@progress/kendo-angular-grid";
import { IntlModule } from "@progress/kendo-angular-intl";
import { ButtonModule, ButtonsModule } from "@progress/kendo-angular-buttons";
import { InputsModule } from "@progress/kendo-angular-inputs";

import { FormListSelectComponent } from "./form-list-select.component";
import { OfficeSharedModule } from "../../shared/office-shared.module";

@NgModule({
    imports: [CommonModule, FormsModule, RouterModule, OfficeSharedModule, GridModule, ButtonModule, ButtonsModule, InputsModule, IntlModule],
    exports: [FormListSelectComponent],
    declarations: [FormListSelectComponent]
})
export class FormListSelectModule { }
