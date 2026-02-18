import { Component } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { Location as Location2 } from "@angular/common";

import { DialogContentBase, DialogRef } from "@progress/kendo-angular-dialog";
import { filter } from "rxjs";
import { IFormlyFormInputs } from "@officeNg";

@Component({
    selector: "myData-detail-dialog",
    templateUrl: "./myData-detail-dialog.html"
})
export class  MyDataDetailDialogComponent extends DialogContentBase {
    title: string;
    private _inputs: IFormlyFormInputs

    get inputs() {
        return this._inputs;
    }
    set inputs(value: IFormlyFormInputs) {
        this._inputs = value;
        this.afterSetValue(value);
    };

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

    afterSetValue(inputs: IFormlyFormInputs) {
    }
    filterInputsChange(filterInputs: IFormlyFormInputs) {

        //filterInputs.properties['saveState']['expressions'] = {
        //    'props.disabled': (field: FormlyFieldConfig) => {
        //        return !field.form.dirty;
        //    }
        //};

        //filterInputs.properties['saveState'].props['change'] = () => {
        //    this.gridView.filterSaveState(filterInputs);
        //};

        //filterInputs.properties['removeState'].props['click'] = () => {
        //    this.gridView.filterRemoveState(filterInputs);
        //};

        //this.filterInputs = filterInputs
    }

}
