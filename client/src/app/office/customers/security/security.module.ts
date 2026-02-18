import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { OfficeSharedModule } from '@officeNg';
import { SecurityRoutingModule } from './security-routing.module';
import { SecurityComponent } from './components/security.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        SecurityRoutingModule
    ],
    declarations: [
        SecurityComponent
    ]
})
export class SecurityModule { }
