import { Component } from "@angular/core";
import { BookmarkUnitOfWork } from "@officeNg";

@Component({
    selector: "bookmark-list",
    templateUrl: "./bookmark-list.html"
})
export class BookmarkListComponent {
    pathUrl = 'office/bookmark';
    constructor(public uow: BookmarkUnitOfWork) { }
}
