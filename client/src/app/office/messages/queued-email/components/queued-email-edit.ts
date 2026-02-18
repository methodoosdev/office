import { Component, ViewChild } from "@angular/core";

import { FormlyEditNewToken, QueuedEmailUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "queued-email-edit",
    templateUrl: "./queued-email-edit.html"
})
export class QueuedEmailEditComponent implements CanComponentDeactivate {
    @ViewChild(FormlyEditNewToken, { static: true }) editForm: FormlyEditNewToken | null = null;
    parentUrl = 'office/queued-email';

    constructor(
        public uow: QueuedEmailUnitOfWork) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.editForm.canDeactivate();
    }
}
