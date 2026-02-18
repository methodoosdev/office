import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppConfigComponent } from './app.config.component';
import { ButtonModule, InputSwitchModule, RadioButtonModule, SidebarModule, TooltipModule } from '@primeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SidebarModule,
        RadioButtonModule,
        ButtonModule,
        InputSwitchModule,
        TooltipModule
    ],
    declarations: [
        AppConfigComponent
    ],
    exports: [
        AppConfigComponent
    ]
})
export class AppConfigModule { }
