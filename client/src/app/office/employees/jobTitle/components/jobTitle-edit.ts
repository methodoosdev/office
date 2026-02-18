import { Component, ViewChild } from "@angular/core";
import { JobTitleUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "jobTitle-edit",
    templateUrl: "./jobTitle-edit.html"
})
export class JobTitleEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/jobTitle';

    constructor(public uow: JobTitleUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
