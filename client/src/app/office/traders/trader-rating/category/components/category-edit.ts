import { Component, ViewChild } from "@angular/core";
import { TraderRatingCategoryUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "trader-rating-category-edit",
    templateUrl: "./category-edit.html"
})
export class TraderRatingCategoryEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/trader-rating-category';

    constructor(public uow: TraderRatingCategoryUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
