import { Component, ViewChild } from "@angular/core";
import { FormlyEditNewToken, TraderMonthlyBillingUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "trader-monthly-billing-edit",
    templateUrl: "./trader-monthly-billing-edit.html"
})
export class TraderMonthlyBillingEditComponent implements CanComponentDeactivate {
    @ViewChild(FormlyEditNewToken, { static: true }) editForm: FormlyEditNewToken | null = null;
    parentUrl = 'office/trader';

    constructor(public uow: TraderMonthlyBillingUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.editForm.canDeactivate();
    }
}
