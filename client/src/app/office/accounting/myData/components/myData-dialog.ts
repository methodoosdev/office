import { Component } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { Location as Location2 } from "@angular/common";

import { DialogContentBase, DialogRef } from "@progress/kendo-angular-dialog";
import { filter } from "rxjs";
import { IFormlyFormInputs } from "@officeNg";
import { FormlyFieldConfig } from "@ngx-formly/core";

@Component({
    selector: "myData-dialog",
    templateUrl: "./myData-dialog.html"
})
export class  MyDataDialogComponent extends DialogContentBase {
    title: string;
    private _inputs: IFormlyFormInputs

    get inputs() {
        return this._inputs;
    }
    set inputs(value: IFormlyFormInputs) {
        this._inputs = value;
        this.afterSetValue(value);
    };

    afterSetValue(inputs: IFormlyFormInputs) {
        const properties = inputs.properties;
        const customProperties = inputs.customProperties;

        //inputs.properties['docTypeId'].props['change'] = (field: FormlyFieldConfig, event: any) => {
        //    this.gridView.filterSaveState(filterInputs);
        //};

    }

    constructor(location: Location2, router: Router, dialogRef: DialogRef) {
        super(dialogRef);

        router.events.pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(() => {
                const path = location.path();
                if (path != "/myData") {
                    this.dialog.close();
                } 
            });
    }
}
