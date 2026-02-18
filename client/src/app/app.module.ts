import { LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { IconSettingsService } from '@progress/kendo-angular-icons';
import { ICON_SETTINGS } from '@progress/kendo-angular-icons';
import { MessageService } from "@progress/kendo-angular-l10n";
import { LoadingBarModule } from '@ngx-loading-bar/core';
import { FormlyModule } from '@ngx-formly/core';
import { ToastrModule } from 'ngx-toastr';

import { CoreModule } from './core/core.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CustomMessageService } from './core/services/custom-message.service';
import { ErrorHandler } from './core/services/error-handler';
import { CustomExceptionHandlerProvider } from './core/services/custom-exception-handler';
import { ImageDialogModule, OfficeSharedModule } from '@officeNg';
//import { AppLayoutModule } from './core/components';

import 'hammerjs';

import '@progress/kendo-angular-intl/locales/el/all';
import '@progress/kendo-angular-intl/locales/en/all';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        CoreModule,
        AppRoutingModule,
        //AppLayoutModule, // Avalon,
        LoadingBarModule,
        ToastrModule.forRoot({
            closeButton: true,
            progressBar: true,
            positionClass: 'toast-bottom-right'
        }),
        OfficeSharedModule.forRoot(),
        FormlyModule.forRoot(),
        ImageDialogModule
    ],
    providers: [
        ErrorHandler,
        CustomExceptionHandlerProvider,
        { provide: MessageService, useClass: CustomMessageService },
        { provide: LOCALE_ID, useValue: 'el-GR' },
        IconSettingsService,
        { provide: ICON_SETTINGS, useValue: { type: 'font' } }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
