import { Component, OnInit } from "@angular/core";
import { CustomerActivityUnitOfWork } from "@officeNg";

@Component({
    selector: "customer-activity-log",
    templateUrl: "./customer-activity-log.html"
})
export class CustomerActivityLogComponent implements OnInit {
    content: string;

    constructor(private uow: CustomerActivityUnitOfWork) {
    }

    ngOnInit(): void {
        this.uow.lastActivity()
            .then((result) => {
                this.content = result.comment;
            });
    }
}
