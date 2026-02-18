import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { FmyContributionRoutingModule } from "./fmy-contribution-routing.module";
import { FmyContributionComponent } from "./components/fmy-contribution";
import { FormlyEditModule, FmyContributionUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FmyContributionRoutingModule,
        FormlyEditModule,
        StickyModule
    ],
    declarations: [
        FmyContributionComponent
    ],
    providers: [
        FmyContributionUnitOfWork
    ],
})
export class FmyContributionModule { }
