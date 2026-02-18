import { Component, ViewChild } from "@angular/core";
import { SimpleTaskCategoryUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "simple-task-category-edit",
    templateUrl: "./simple-task-category-edit.html"
})
export class SimpleTaskCategoryEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/simple-task-category';

    constructor(public uow: SimpleTaskCategoryUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
