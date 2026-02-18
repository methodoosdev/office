import { Component } from "@angular/core";
import { ScriptTableNameUnitOfWork } from "@officeNg";

@Component({
    selector: "script-table-name-list",
    templateUrl: "./table-name-list.html"
})
export class ScriptTableNameListComponent {
    pathUrl = 'office/script-table-name';
    constructor(public uow: ScriptTableNameUnitOfWork) { }
}
