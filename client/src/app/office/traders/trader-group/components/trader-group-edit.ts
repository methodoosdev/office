import { Component, ViewChild } from "@angular/core";
import { TraderGroupUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "trader-group-edit",
    templateUrl: "./trader-group-edit.html"
})
export class TraderGroupEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/trader-group';

    constructor(public uow: TraderGroupUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
