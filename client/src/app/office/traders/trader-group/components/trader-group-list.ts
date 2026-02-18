import { Component } from "@angular/core";
import { TraderGroupUnitOfWork } from "@officeNg";

@Component({
    selector: "trader-group-list",
    templateUrl: "./trader-group-list.html"
})
export class TraderGroupListComponent {
    pathUrl = 'office/trader-group';
    constructor(public uow: TraderGroupUnitOfWork) { }
}
