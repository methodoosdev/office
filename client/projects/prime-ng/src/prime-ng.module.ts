import { NgModule } from '@angular/core';

import { BadgeModule } from './badge/badge';
import { AvatarModule } from './avatar/avatar';
import { ButtonModule } from './button/button';
import { CheckboxModule } from './checkbox/checkbox';
import { InputSwitchModule } from './inputswitch/inputswitch';
import { InputTextModule } from './inputtext/inputtext';
import { MenuModule } from './menu/menu';
import { RadioButtonModule } from './radiobutton/radiobutton';
import { RippleModule } from './ripple/ripple';
import { ScrollTopModule } from './scrolltop/scrolltop';
import { SidebarModule } from './sidebar/sidebar';
import { StyleClassModule } from './styleclass/styleclass';
import { ToolbarModule } from './toolbar/toolbar';
import { TooltipModule } from './tooltip/tooltip';
import { PrimeSharedModule } from './api/shared';

@NgModule({
    imports: [
        AvatarModule,
        BadgeModule,
        ButtonModule,
        CheckboxModule,
        InputSwitchModule,
        InputTextModule,
        MenuModule,
        RadioButtonModule,
        RippleModule,
        ScrollTopModule,
        SidebarModule,
        StyleClassModule,
        ToolbarModule,
        TooltipModule,
        PrimeSharedModule
    ],
    exports: [
        AvatarModule,
        BadgeModule,
        ButtonModule,
        CheckboxModule,
        InputSwitchModule,
        InputTextModule,
        MenuModule,
        RadioButtonModule,
        RippleModule,
        ScrollTopModule,
        SidebarModule,
        StyleClassModule,
        ToolbarModule,
        TooltipModule,
        PrimeSharedModule
    ]
})
export class PrimeNgModule { }
