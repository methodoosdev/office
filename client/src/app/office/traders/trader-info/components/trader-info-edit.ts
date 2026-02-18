import { Component, ViewChild } from "@angular/core";
import { FormlyEditNewToken, TraderInfoUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "trader-info-edit",
    templateUrl: "./trader-info-edit.html"
})
export class TraderInfoEditComponent implements CanComponentDeactivate {
    @ViewChild(FormlyEditNewToken, { static: true }) editForm: FormlyEditNewToken | null = null;
    parentUrl = 'office/trader';

    constructor(public uow: TraderInfoUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.editForm.canDeactivate();
    }
}
