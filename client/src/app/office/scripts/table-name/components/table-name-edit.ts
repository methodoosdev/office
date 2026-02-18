import { Component, ViewChild } from "@angular/core";
import { ScriptTableNameUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "script-table-name-edit",
    templateUrl: "./table-name-edit.html"
})
export class ScriptTableNameEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/script-table-name';

    constructor(public uow: ScriptTableNameUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
