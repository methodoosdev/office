import { Component } from "@angular/core";
import { TraderRatingUnitOfWork } from "@officeNg";

@Component({
    selector: "trader-rating-list",
    templateUrl: "./rating-list.html"
})
export class TraderRatingListComponent {
    pathUrl = 'office/trader-rating';
    constructor(public uow: TraderRatingUnitOfWork) { }
}
