import { Component, ViewChild } from "@angular/core";
import { MyDataItemUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "myData-item-edit",
    templateUrl: "./myData-item-edit.html"
})
export class MyDataItemEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/myData-item';

    constructor(public uow: MyDataItemUnitOfWork) { }
    
    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
