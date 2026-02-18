import { CommonModule } from "@angular/common";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { APP_INITIALIZER, NgModule, Optional, SkipSelf } from "@angular/core";
import { RouterModule } from "@angular/router";

import { GridModule, ExcelModule as GridExcelModule, PDFModule } from "@progress/kendo-angular-grid";
import { DropDownsModule } from "@progress/kendo-angular-dropdowns";
import { InputsModule } from "@progress/kendo-angular-inputs";
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
import { MenusModule } from "@progress/kendo-angular-menu";
import { PopupModule } from "@progress/kendo-angular-popup";
import { ProgressBarModule } from "@progress/kendo-angular-progressbar";

import { ApiConfigService } from "./services/api-config.service";
import { AuthInterceptor, XsrfInterceptor } from "@jwtNg";

@NgModule({
    imports: [
        CommonModule, RouterModule,

        GridModule, GridExcelModule, PDFModule,
        DropDownsModule,
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
        TreeListModule, TreeListExcelModule,
        ExcelExportModule, MenusModule, PopupModule,
        ProgressBarModule],
    exports: [
        // components that are used in app.component.ts will be listed here.
        GridModule, GridExcelModule, PDFModule,
        DropDownsModule,
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
        TreeListModule, TreeListExcelModule,
        ExcelExportModule, MenusModule, PopupModule,
        ProgressBarModule
    ],
    declarations: [
        // components that are used in app.component.ts will be listed here.
    ],

    providers: [
        /* ``No`` global singleton services of the whole app should be listed here anymore!
           Since they'll be already provided in AppModule using the `tree-shakable providers` of Angular 6.x+ (providedIn: 'root').
           This new feature allows cleaning up the providers section from the CoreModule.
           But if you want to provide something with an InjectionToken other that its class, you still have to use this section.
        */
        {
            provide: HTTP_INTERCEPTORS,
            useClass: XsrfInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        {
            provide: APP_INITIALIZER,
            useFactory: (config: ApiConfigService) => () => config.loadApiConfig(),
            deps: [ApiConfigService],
            multi: true
        }
    ]
})
export class CoreModule {
    constructor(@Optional() @SkipSelf() core: CoreModule) {
        if (core) {
            throw new Error("CoreModule should be imported ONLY in AppModule.");
        }
    }
}
