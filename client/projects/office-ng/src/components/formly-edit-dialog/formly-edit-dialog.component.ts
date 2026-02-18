import { ChangeDetectorRef, Component, Input, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { DialogContentBase, DialogRef } from "@progress/kendo-angular-dialog";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { fixKendoDateTime, getFieldProperties } from "@formlyNg";
import { PrimeNGConfig } from "@primeNg";
@Component({
    selector: "formly-edit-dialog",
    template: `
        <kendo-dialog-titlebar>
            <div style="font-size: 18px; line-height: 1.3em;">
                <span>{{title}}</span>
            </div>
        </kendo-dialog-titlebar>

        <form [formGroup]="form" class="k-form" [ngClass]="{'k-form-horizontal': horizontal,'k-form-vertical': !horizontal}">
            <formly-form [form]="form" [model]="model" [fields]="fields" [options]="options"></formly-form>
        </form>

        <kendo-dialog-actions layout="end">
            <button kendoButton (click)="onConfirmAction()" themeColor="primary" [disabled]="!form.valid">{{saveLabel}}</button>
            <button kendoButton (click)="onCancelAction()">{{cancelLabel}}</button>
        </kendo-dialog-actions>
  `
})
export class FormlyEditDialogComponent extends DialogContentBase implements OnInit {
    @Input() horizontal = false;
    public form = new FormGroup({});
    public options: FormlyFormOptions = {};
    public fields: FormlyFieldConfig[];
    public model: any;

    public title!: string;
    public createLabel: boolean = false;
    public saveLabel!: string;
    public cancelLabel!: string;

    customProperties: any;
    fieldProperties: { [key: string]: FormlyFieldConfig };

    constructor(dialogRef: DialogRef, private primengConfig: PrimeNGConfig, private ref: ChangeDetectorRef) {
        super(dialogRef);
    }

    ngOnInit(): void {
        this.saveLabel = this.primengConfig.getTranslation(this.createLabel ? "common.create" : 'common.save');
        this.cancelLabel = this.primengConfig.getTranslation('common.cancel');
    }

    public setModel(data: any) {
        const fieldProperties = getFieldProperties(data.formModel.customProperties.fields);
        const pureModel = fixKendoDateTime(data.model, fieldProperties);

        this.model = pureModel;

        this.customProperties = data.formModel.customProperties;
        this.fieldProperties = fieldProperties;
        this.fields = data.formModel.customProperties.fields;
        this.title = this.primengConfig.getTranslation(this.model.id === 0 ? 'common.insert' : 'common.modify');
    }
    
    public onCancelAction(): void {
        this.dialog.close({ text: "Cancel" });
    }

    public onConfirmAction(): void {
        this.dialog.close({ text: "Submit" });
    }
}
