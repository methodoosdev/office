import { Component } from "@angular/core";
import { TraderRatingCategoryUnitOfWork } from "@officeNg";

@Component({
    selector: "trader-rating-category-list",
    templateUrl: "./category-list.html"
})
export class TraderRatingCategoryListComponent {
    pathUrl = 'office/trader-rating-category';
    constructor(public uow: TraderRatingCategoryUnitOfWork) { }
}
