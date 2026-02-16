import { NgModule } from "@angular/core";

import { GridModule, ExcelModule as GridExcelModule, PDFModule } from "@progress/kendo-angular-grid";
import { DropDownsModule } from "@progress/kendo-angular-dropdowns";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { DiagramModule } from "@progress/kendo-angular-diagrams";
import { DateInputsModule } from "@progress/kendo-angular-dateinputs";
import { LabelModule } from "@progress/kendo-angular-label";
import { DialogModule, DialogsModule } from "@progress/kendo-angular-dialog";
import { ButtonModule, ButtonsModule } from "@progress/kendo-angular-buttons";
import { IndicatorsModule } from "@progress/kendo-angular-indicators";
import { CardModule, TabStripModule, LayoutModule } from "@progress/kendo-angular-layout";
import { ListViewModule } from "@progress/kendo-angular-listview";
import { ToolBarModule } from "@progress/kendo-angular-toolbar";
import { IntlModule } from "@progress/kendo-angular-intl";
import { NotificationModule } from "@progress/kendo-angular-notification";
import { TreeListModule, ExcelModule as TreeListExcelModule } from "@progress/kendo-angular-treelist";
import { ExcelExportModule } from "@progress/kendo-angular-excel-export";
import { PDFExportModule } from "@progress/kendo-angular-pdf-export";
import { MenusModule } from "@progress/kendo-angular-menu";
import { PopupModule } from "@progress/kendo-angular-popup";
import { ProgressBarModule } from "@progress/kendo-angular-progressbar";
import { PivotGridModule } from "@progress/kendo-angular-pivotgrid";
import { SVGIconModule, IconsModule } from "@progress/kendo-angular-icons";


@NgModule({
    imports: [
        GridModule, GridExcelModule, PDFModule,
        DropDownsModule,
        DiagramModule,
        DialogModule,
        DialogsModule,
        ButtonModule, ButtonsModule,
        InputsModule,
        DateInputsModule,
        LabelModule,
        IndicatorsModule,
        CardModule,
        TabStripModule,
        LayoutModule,
        ListViewModule,
        ToolBarModule,
        IntlModule,
        NotificationModule,
        PivotGridModule,
        TreeListModule, TreeListExcelModule,
        ExcelExportModule, PDFExportModule,
        MenusModule, PopupModule,
        ProgressBarModule,
        SVGIconModule,
        IconsModule
    ],
    exports: [
        GridModule, GridExcelModule, PDFModule,
        DropDownsModule,
        DiagramModule,
        DialogModule,
        DialogsModule,
        ButtonModule, ButtonsModule,
        InputsModule,
        DateInputsModule,
        LabelModule,
        IndicatorsModule,
        CardModule,
        TabStripModule,
        LayoutModule,
        ListViewModule,
        ToolBarModule,
        IntlModule,
        NotificationModule,
        PivotGridModule,
        TreeListModule, TreeListExcelModule,
        ExcelExportModule, PDFExportModule,
        MenusModule, PopupModule,
        ProgressBarModule,
        SVGIconModule,
        IconsModule
    ]
})
export class KendoSharedModule { }
