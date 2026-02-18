import { Component, ViewChild } from "@angular/core";
import { BookmarkUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "bookmark-edit",
    templateUrl: "./bookmark-edit.html"
})
export class BookmarkEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/bookmark';

    constructor(public uow: BookmarkUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
