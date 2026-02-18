import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { ApdContributionRoutingModule } from "./apd-contribution-routing.module";
import { ApdContributionComponent } from "./components/apd-contribution";
import { FormlyEditModule, ApdContributionUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ApdContributionRoutingModule,
        FormlyEditModule,
        StickyModule
    ],
    declarations: [
        ApdContributionComponent
    ],
    providers: [
        ApdContributionUnitOfWork
    ],
})
export class ApdContributionModule { }
