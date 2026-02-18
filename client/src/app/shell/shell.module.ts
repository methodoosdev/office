import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PrimeNgModule } from '@primeNg';

import { WelcomeComponent } from './welcome/welcome.component';

import { ShellRoutingModule } from './shell-routing.module';
import { LandingComponent } from './landing/landing.component';
import { ShellLayoutComponent } from './layout/shell-layout.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        PrimeNgModule,
        ShellRoutingModule
    ],
    declarations: [
        ShellLayoutComponent,
        LandingComponent,
        WelcomeComponent
    ]
})
export class ShellModule { }
