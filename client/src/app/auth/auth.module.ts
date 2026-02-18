import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PrimeNgModule } from '@primeNg';

import { AccessDeniedComponent } from "./access-denied/access-denied.component";
import { ChangePasswordComponent } from "./change-password/change-password.component";
import { LoginComponent } from './login/login.component';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthLayoutComponent } from './layout/auth-layout.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        AuthRoutingModule,
        PrimeNgModule
    ],
    declarations: [
        AuthLayoutComponent,
        LoginComponent,
        AccessDeniedComponent,
        ChangePasswordComponent
    ]
})
export class AuthModule { }
