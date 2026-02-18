import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { ArticlesCheckRoutingModule } from "./articles-check-routing.module";
import { ArticlesCheckListComponent } from "./components/articles-check-list";
import { ArticlesCheckViewComponent } from "./components/articles-check-view";
import { FormListModule, FormlyEditModule, ArticlesCheckUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        StickyModule,
        ArticlesCheckRoutingModule
    ],
    declarations: [
        ArticlesCheckListComponent,
        ArticlesCheckViewComponent
    ],
    providers: [
        ArticlesCheckUnitOfWork
    ],
})
export class ArticlesCheckModule { }
