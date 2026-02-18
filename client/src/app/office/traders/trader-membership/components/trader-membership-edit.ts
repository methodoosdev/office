import { Component, ViewChild } from "@angular/core";
import { FormlyEditNewToken, TraderMembershipUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "trader-membership-edit",
    templateUrl: "./trader-membership-edit.html"
})
export class TraderMembershipEditComponent implements CanComponentDeactivate {
    @ViewChild(FormlyEditNewToken, { static: true }) editForm: FormlyEditNewToken | null = null;
    parentUrl = 'office/trader';

    constructor(public uow: TraderMembershipUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.editForm.canDeactivate();
    }
}
