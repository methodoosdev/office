import { Component, ViewChild } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { FormGroup } from "@angular/forms";
import { Location as Location2 } from "@angular/common";

import { DialogContentBase, DialogRef } from "@progress/kendo-angular-dialog";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { filter, Subscription, distinctUntilChanged, map, startWith } from "rxjs";

import { FieldType, FormlyFieldGridSelect } from "@formlyNg";
import { TranslationService } from "@core";
import { AfterModelChangeEvent } from "@officeNg";

type Status = 'retrieve' | 'generate' | 'identity' | 'representative';

export class PeriodicF2Dialog {
    constructor(
        public status: Status,
        public connectionId: string,
        public traderId: number = 0,
        public date: Date = new Date(),
        public period: number = 0,
        public f007: boolean = false,
        public representative: boolean = false) {
    }
}
@Component({
    selector: "periodic-f2-dialog",
    templateUrl: "./periodic-f2-dialog.html"
})
export class PeriodicF2DialogComponent extends DialogContentBase {
    @ViewChild(FormlyFieldGridSelect, { static: false }) select!: FormlyFieldGridSelect;
    private fireEventSub: Subscription;

    title: string;

    form = new FormGroup({});
    options: FormlyFormOptions = {};
    fields: FormlyFieldConfig[];
    model: any;

    constructor(location: Location2, router: Router, dialogRef: DialogRef, 
        private translationService: TranslationService) {
        super(dialogRef);

        router.events.pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(() => {
                const path = location.path();
                if (path != "/periodic-f2") {
                    this.dialog.close();
                } 
            });
    }

    initialize(dialogModel: any, titlePart: string, status: Status, connectionId: string) {
        const title = dialogModel.customProperties.title;
        const fields: FormlyFieldConfig[] = dialogModel.customProperties.fields;
        const periodsC = dialogModel.customProperties.periodsC;
        const periodsB = dialogModel.customProperties.periodsB;

        this.options = {
            formState: {
                categoryBook: null,
                periodsC: periodsC,
                periodsB: periodsB,
                status: status
            }
        };

        if (status !== 'representative') {
            const periodProperty = fields.find(x => x.key === 'period');
            periodProperty.hooks = {
                onInit: (field: FormlyFieldConfig) => {

                    const periodControl = field.parent.get('period').formControl;
                    const traderIdControl = field.parent.get('traderId').formControl;
                    field.props.options = traderIdControl.valueChanges.pipe(
                        startWith(traderIdControl.value),
                        distinctUntilChanged(),
                        map(() => {
                            periodControl.setValue(0);
                            if (traderIdControl.value === 0) {
                                return [];
                            }
                            const formState = field.parent.options.formState
                            if (formState.categoryBook === true) { // if categoryBook true B'category book
                                return formState.periodsB
                            } else if (formState.categoryBook === false) {
                                return formState.periodsC;
                            } else {
                                return [];
                            }
                        }),
                    );
                },
            };
        }

        this.model = new PeriodicF2Dialog(status, connectionId);
        this.title = `${title} - ${titlePart}`;
        this.fields = [{
            fieldGroupClassName: "grid",
            fieldGroup: fields
        }];

        const traderIdProperty = fields.find(x => x.key === 'traderId');
        traderIdProperty.props['selectionChange'] = (field: FormlyFieldConfig, dataItem: any) => {

            if (dataItem) {
                this.options.formState["categoryBook"] = dataItem.disabled;
            } else {
                this.options.formState["categoryBook"] = null;
            }
        };
    }
}
