import { Component, ViewChild } from "@angular/core";
import { CustomerPermissionUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "customer-permission-edit",
    templateUrl: "./customer-permission-edit.html"
})
export class CustomerPermissionEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/customer-permission';

    constructor(public uow: CustomerPermissionUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
