import { Component } from "@angular/core";

import { QueuedEmailUnitOfWork } from "@officeNg";
import { ChatHubService } from "@core";

@Component({
    selector: "queued-email-list",
    templateUrl: "./queued-email-list.html",
    providers: [ChatHubService]
})
export class QueuedEmailListComponent {
    pathUrl = 'office/queued-email';

    constructor(
        public uow: QueuedEmailUnitOfWork) {
    }
}
