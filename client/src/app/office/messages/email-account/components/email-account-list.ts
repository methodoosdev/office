import { Component } from "@angular/core";
import { EmailAccountUnitOfWork } from "@officeNg";

@Component({
    selector: "email-account-list",
    templateUrl: "./email-account-list.html"
})
export class EmailAccountListComponent {
    pathUrl = 'office/email-account';
    constructor(public uow: EmailAccountUnitOfWork) { }
}
