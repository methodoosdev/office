import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { lastValueFrom } from "rxjs";

import { RowClassArgs } from "@progress/kendo-angular-grid";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { saveAs } from '@progress/kendo-file-saver';
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";

import { ToastrService } from "ngx-toastr";

import { ProgressHubService, TranslationService } from "@core";
import { ColumnButtonClickEvent, DialogResult, GridViewToken, PeriodicF2UnitOfWork, ColumnSetting } from "@officeNg";
import { PeriodicF2Dialog, PeriodicF2DialogComponent } from "./periodic-f2-dialog";

@Component({
    selector: "periodic-f2-list",
    templateUrl: "./periodic-f2-list.html",
    providers: [ProgressHubService]
})
export class PeriodicF2ListComponent implements OnInit, OnDestroy {
    pathUrl = 'office/periodic-f2';
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];

    title: string;
    createLabel: string;
    deleteLabel: string;
    retrieveLabel: string;
    identityPaymentLabel: string;
    submitLabel: string;

    dialogPropertiesFromServer: any;
    actions: any[];

    constructor(
        private translationService: TranslationService,
        public uow: PeriodicF2UnitOfWork,
        private toastrService: ToastrService,
        private dialogService: DialogService,
        private hubService: ProgressHubService) {

        this.title = this.translationService.translate('menu.periodic-f2');
        this.identityPaymentLabel = this.translationService.translate('common.identityPayment');
        this.retrieveLabel = this.translationService.translate('common.retrieve');
        this.createLabel = this.translationService.translate('common.create');
        this.deleteLabel = this.translationService.translate('common.delete');
        this.submitLabel = this.translationService.translate('common.submit');

        const yesLabel = this.translationService.translate('common.yes');
        const noLabel = this.translationService.translate('common.cancel');

        this.actions = [
            { text: yesLabel, themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: noLabel, dialogResult: DialogResult.Cancel }
        ];
    }

    rowClass = (args: RowClassArgs) => ({
        "yellow": (args.dataItem["submitModeTypeId"] == 2)
    });

    columnButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'submit') {

            const dialogRef: DialogRef = this.dialogService.open({
                content: PeriodicF2DialogComponent,
                actions: this.actions,
                width: 550, height: 160, minWidth: 160, actionsLayout: 'end',
                preventAction: (e: any, dialog) => {
                    return this.preventAction(e, dialog);
                }
            });

            const dialog = dialogRef.content.instance as PeriodicF2DialogComponent;
            dialog.initialize(this.dialogPropertiesFromServer, this.submitLabel, 'representative', this.hubService?.connectionId);

            lastValueFrom(dialogRef.result).then((result: any) => {
                if (result.dialogResult === DialogResult.Ok) {

                    this.uow.submit(event.dataItem.id, dialog.model.representative, this.hubService?.connectionId)
                        .then((result: Blob) => {
                            if (result.size > 0)
                                saveAs(result, "periodicF2.png");
                            else
                                this.toastrService.error(this.translationService.translate('error.downloadImageFailed'));
                        })
                        .catch((err: HttpErrorResponse) => {
                            const reader = new FileReader();
                            reader.onloadend = () => {
                                const base64data = reader.result as string;
                                this.toastrService.error(base64data);
                            }

                            reader.readAsText(err.error);
                        });
                }
            });
        }
    }

    cellButtonDisabled(column: ColumnSetting, dataItem: any) {
        if (column.field === "submit") {
            if (dataItem["submitModeTypeId"] == 1) {
                return "k-disabled";
            }
        }
        return "";
    }

    ngOnDestroy(): void {
        //this.hubService.stop();
    }

    ngOnInit(): void {
        //this.hubService.start();

        this.settingsMenuItems = [
            {
                text: this.translationService.translate('common.saveState'),
                click: () => {
                    this.table.saveState();
                }
            }, {
                text: this.translationService.translate('common.removeState'),
                click: () => {
                    this.table.removeState();
                }
            }];
    }

    private preventAction(event: any, dialog?: DialogRef): boolean {
        if (event.dialogResult === DialogResult.Ok) {
            const model: PeriodicF2Dialog = (dialog.content.instance as PeriodicF2DialogComponent).model;

            if (model.status === 'representative')
                return false;
            if (model.traderId == 0) {
                this.toastrService.warning(this.translationService.translate('message.traderNotEmpty'));
                return true;
            } else if (model.period == 0) {
                this.toastrService.warning(this.translationService.translate('message.periodNotEmpty'));
                return true;
            } else
                return false;
        } else
            return false;
    }

    retrieve() {
        const dialogRef: DialogRef = this.dialogService.open({
            content: PeriodicF2DialogComponent,
            actions: this.actions,
            width: 550, height: 360, minWidth: 360, actionsLayout: 'end',
            preventAction: (e: any, dialog) => {
                return this.preventAction(e, dialog);
            }
        });

        const dialog = dialogRef.content.instance as PeriodicF2DialogComponent;
        dialog.initialize(this.dialogPropertiesFromServer, this.retrieveLabel, 'retrieve', this.hubService?.connectionId);

        lastValueFrom(dialogRef.result).then((result: any) => {
            if (result.dialogResult === DialogResult.Ok) {

                this.uow.retrieve(dialog.model, this.hubService?.connectionId)
                    .then((res) => {
                        this.table.refresh();
                    });
            }
        });
    }

    generate() {
        const dialogRef: DialogRef = this.dialogService.open({
            content: PeriodicF2DialogComponent,
            actions: this.actions,
            width: 550, height: 360, minWidth: 360, actionsLayout: 'end',
            preventAction: (e: any, dialog) => {
                return this.preventAction(e, dialog);
            }
        });

        const dialog = dialogRef.content.instance as PeriodicF2DialogComponent;
        dialog.initialize(this.dialogPropertiesFromServer, this.createLabel, 'generate', this.hubService?.connectionId);

        lastValueFrom(dialogRef.result).then((result: any) => {
            if (result.dialogResult === DialogResult.Ok) {

                this.uow.generate(dialog.model, this.hubService?.connectionId)
                    .then((res) => {
                        this.table.refresh();
                    });
            }
        });
    }

    identityPayment() {
        const dialogRef: DialogRef = this.dialogService.open({
            content: PeriodicF2DialogComponent,
            actions: this.actions,
            width: 550, height: 360, minWidth: 360, actionsLayout: 'end',
            preventAction: (e: any, dialog) => {
                return this.preventAction(e, dialog);
            }
        });

        const dialog = dialogRef.content.instance as PeriodicF2DialogComponent;
        dialog.initialize(this.dialogPropertiesFromServer, this.createLabel, 'identity', this.hubService?.connectionId);

        lastValueFrom(dialogRef.result).then((result: any) => {
            if (result.dialogResult === DialogResult.Ok) {

                this.uow.identityPayment(dialog.model, this.hubService?.connectionId)
                    .catch((error: HttpErrorResponse) => {
                        this.toastrService.error(this.translationService.translate('error.downloadImageFailed'));
                    });
            }
        });
    }

    deleteRecord() {
        this.table.deleteRecord();
    }

    //dialog
    onLoadProperties(result: any) {
        this.dialogPropertiesFromServer = result.dialogModel;
    }
}
