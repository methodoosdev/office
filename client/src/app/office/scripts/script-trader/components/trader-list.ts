import { Component } from "@angular/core";
import { ScriptTraderUnitOfWork } from "@officeNg";

@Component({
    selector: "script-trader-list",
    templateUrl: "./trader-list.html"
})
export class ScriptTraderListComponent {
    pathUrl = 'office/script-trader';
    constructor(public uow: ScriptTraderUnitOfWork) { }
}
