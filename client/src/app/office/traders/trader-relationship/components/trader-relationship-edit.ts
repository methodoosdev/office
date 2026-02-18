import { Component, ViewChild } from "@angular/core";
import { FormlyEditNewToken, TraderRelationshipUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "trader-relationship-edit",
    templateUrl: "./trader-relationship-edit.html"
})
export class TraderRelationshipEditComponent implements CanComponentDeactivate {
    @ViewChild(FormlyEditNewToken, { static: true }) editForm: FormlyEditNewToken | null = null;
    parentUrl = 'office/trader';

    constructor(public uow: TraderRelationshipUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.editForm.canDeactivate();
    }
}
