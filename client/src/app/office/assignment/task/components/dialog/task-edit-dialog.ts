import { Component } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { Location as Location2 } from "@angular/common";
import { NavigationEnd, Router } from "@angular/router";
import { filter } from "rxjs";
import { DialogContentBase, DialogRef } from "@progress/kendo-angular-dialog";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";

@Component({
    selector: "task-edit-dialog",
    templateUrl: "./task-edit-dialog.html"
})
export class TaskEditDialogComponent extends DialogContentBase {

    title: string;

    form = new FormGroup({});
    options: FormlyFormOptions = {};
    fields: FormlyFieldConfig[];
    model: any;

    constructor(location: Location2, router: Router, dialogRef: DialogRef) {
        super(dialogRef);

        router.events.pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(() => {
                const path = location.path();
                if (path != "/assignment-task") {
                    this.dialog.close();
                } 
            });
    }

    initialize(dialogModel: any, model: any) {
        const fields: FormlyFieldConfig[] = dialogModel.customProperties.fields;

        this.model = model;
        this.title = dialogModel.customProperties.title;
        this.fields = fields;
    }
}
