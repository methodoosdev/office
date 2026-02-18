import { Component, ViewChild } from "@angular/core";
import { CustomerRoleUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "customer-role-edit",
    templateUrl: "./customer-role-edit.html"
})
export class CustomerRoleEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/customer-role';

    constructor(public uow: CustomerRoleUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
