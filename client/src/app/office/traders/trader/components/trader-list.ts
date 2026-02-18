import { Component, Inject, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from "@angular/core";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { RowClassArgs } from "@progress/kendo-angular-grid";
import { ToastrService } from "ngx-toastr";

import { ActionsMenuService, ChatHubService, TranslationService } from "@core";
import {
    FormListDialogComponent, GridViewToken, SrfTraderUnitOfWork, 
    TraderUnitOfWork, TraderKadUnitOfWork, TraderBranchUnitOfWork,
    TaxSystemTraderUnitOfWork, CheckFhmPosUnitOfWork, IFormlyFormInputs,
    MyDataCredentialsUnitOfWork, TraderBoardMemberUnitOfWork, FinancialObligationUnitOfWork, DialogResult
} from "@officeNg";
import { lastValueFrom } from "rxjs";

@Component({
    selector: "trader-list",
    templateUrl: "./trader-list.html",
    encapsulation: ViewEncapsulation.None,
    styles: [`
        .k-menu:not(.k-context-menu) {
            background-color: #eceff1;
        }
    `],
    providers: [ChatHubService]
})
export class TraderListComponent implements OnInit, OnDestroy {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];

    pathUrl = 'office/trader';
    actionsMenu: any[];
    actionsLabel: string;
    year = new Date();

    filterInputs: IFormlyFormInputs;

    constructor(
        @Inject('BASE_URL') private baseUrl: string,
        private translationService: TranslationService,
        public uow: TraderUnitOfWork,
        public traderKadUow: TraderKadUnitOfWork,
        public traderBranchUow: TraderBranchUnitOfWork,
        public srfTraderUow: SrfTraderUnitOfWork,
        public taxSystemTraderUow: TaxSystemTraderUnitOfWork,
        public checkFhmPosUnitOfWork: CheckFhmPosUnitOfWork,
        public myDataCredentialsUow: MyDataCredentialsUnitOfWork,
        public traderBoardMemberUow: TraderBoardMemberUnitOfWork,
        public financialObligationUow: FinancialObligationUnitOfWork,
        private actionsMenuService: ActionsMenuService,
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private chatHubService: ChatHubService) {
    }

    ngOnDestroy(): void {
        //this.chatHubService.stop();
    }

    ngOnInit(): void {
        //this.chatHubService.start();

        this.settingsMenuItems = [
            {
                text: this.translationService.translate('common.saveState'),
                click: (): void => {
                    this.table.saveState();
                }
            }, {
                text: this.translationService.translate('common.removeState'),
                click: (): void => {
                    this.table.removeState();
                }
            }];

        this.actionsMenu = this.actionsMenuService.traderListActionMenu();
    }

    rowClass = (args: RowClassArgs) => {
        const active: boolean = args.dataItem["active"];
        const deleted: boolean = args.dataItem["deleted"];

        return deleted ? { 'red': true } : !active ? { 'violet': true } : { 'violet red': false };
    };

    onSelectMenu(e: any): void {
        switch (e.item.id) {
            case 'fromSrf':
                this.selectTraderFromSrf();
                break;

            case 'fromTaxSystem':
                this.selectTraderFromTaxSystem();
                break;

            case 'importPayrollIds':
                this.importPayrollIds();
                break;

            case 'importKeaoGredentials':
                this.importKeaoGredentials();
                break;

            case 'importMyDataCredentials':
                this.importMyDataCredentials();
                break;

            case 'checkConnection':
                this.checkConnection();
                break;

            case 'importKadTaxisNet':
                this.importKadTaxisNet();
                break;

            case 'importKadBranchesTaxisNet':
                this.importKadBranchesTaxisNet();
                break;

            case 'efkaNonSalaried':
                this.efkaNonSalaried();
                break;

            case 'createEmailInfoFhmPos':
                this.createEmailInfoFhmPos();
                break;

            case 'undoTraderDeletion':
                this.undoTraderDeletion();
                break;

            case 'importTraderBoardMember':
                this.importTraderBoardMember();
                break;
        }
    }

    filterInputsChange(filterInputs: IFormlyFormInputs) {

        filterInputs.properties['saveState']['expressions'] = {
            'props.disabled': (field: FormlyFieldConfig) => {
                return !field.form.dirty;
            }
        };

        filterInputs.properties['saveState'].props['click'] = () => {
            this.table.filterSaveState(filterInputs);
        };

        filterInputs.properties['removeState'].props['click'] = () => {
            this.table.filterRemoveState(filterInputs);
        };

        this.filterInputs = filterInputs
    }

    selectTraderFromSrf(): void {
        const dialogRef = this.dialogService.open({
            content: FormListDialogComponent,
            width: 1120,
            height: 570
        });

        const component = dialogRef.content.instance as FormListDialogComponent;
        component.title = this.translationService.translate('menu.traders');
        component.uow = this.srfTraderUow;

        dialogRef.result.subscribe((result: any[]) => {
            if (Array.isArray(result)) {
                this.srfTraderUow.import(result)
                    .then(() => {
                        this.table.refresh().then(() => {
                            this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                        });
                    }).catch((error: Error) => {
                        throw error;
                    });
            }
        });
    }

    undoTraderDeletion() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {

            this.uow.undoTraderDeletion(keys)
                .then(() => {
                    this.table.refresh().then(() => {
                        this.toastrService.success(this.translationService.translate('message.modifyCompleted'));
                    });
                }).catch((error: Error) => {
                    throw error;
                });
        } else
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
    }

    importPayrollIds() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {

            this.uow.importPayrollIds(keys)
                .then(() => {
                    this.table.refresh().then(() => {
                        this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                    });
                }).catch((error: Error) => {
                    throw error;
                });
        } else
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
    }

    importKeaoGredentials() {
        this.taxSystemTraderUow.importKeaoGredentials()
            .then(() => {
                this.table.refresh().then(() => {
                    this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                });
            }).catch((error: Error) => {
                throw error;
            });
    }

    importMyDataCredentials() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {

            this.myDataCredentialsUow.importTo(keys, this.chatHubService?.connectionId)
                .then(() => {
                    this.table.refresh().then(() => {
                        this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                    });
                }).catch((error: Error) => {
                    throw error;
                });
        } else
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
    }

    efkaNonSalaried() {

        const actions = [
            { text: this.translationService.translate('common.yes'), themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: this.translationService.translate('common.cancel'), dialogResult: DialogResult.Cancel }
        ];

        const dialog: DialogRef = this.dialogService.open({
            title: this.translationService.translate('common.create'),
            content: this.translationService.translate('trader.efkaNonSalaried'),
            actions: actions,
            width: 450, height: 200, minWidth: 250, actionsLayout: 'end'
        });

        lastValueFrom(dialog.result)
            .then((result: any) => {
                if (result['dialogResult'] === DialogResult.Ok) {

                    this.financialObligationUow.efkaNonSalaried(this.table.GetSelectedKeys(), this.chatHubService?.connectionId)
                        .then(() => {
                            this.table.ClearSelectedKeys();
                            this.table.refresh().then(() => {
                                this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                            });
                        }).catch((error: Error) => {
                            throw error;
                        }).finally(() => {
                            window.open(`${this.baseUrl}docs/customer-activity-log`, "_blank");
                        });
                }
            });
    }

    createEmailInfoFhmPos() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {

            this.checkFhmPosUnitOfWork.createEmail(keys, this.chatHubService?.connectionId)
                .then(() => {
                    this.table.refresh().then(() => {
                        this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                    });
                }).catch((error: Error) => {
                    throw error;
                });
        } else
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
    }

    selectTraderFromTaxSystem(): void {
        const dialogRef = this.dialogService.open({
            content: FormListDialogComponent,
            width: 1120,
            height: 570
        });

        const component = dialogRef.content.instance as FormListDialogComponent;
        component.title = this.translationService.translate('menu.traders');
        component.uow = this.taxSystemTraderUow;

        dialogRef.result.subscribe((result: any[]) => {
            if (Array.isArray(result)) {
                this.taxSystemTraderUow.import(result)
                    .then(() => {
                        this.table.refresh().then(() => {
                            this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                        });
                    }).catch((error: Error) => {
                        throw error;
                    });
            }
        });
    }

    checkConnection() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {
            this.uow.checkConnection(keys)
                .then((result: any) => {
                    if (result.errors && result.errors.length > 0) {
                        const errors = result.errors as any[];
                        let message = "";
                        let count = 1;
                        errors.forEach(x => {
                            const name = x.name as string;
                            message += `<div>${count++}) ${name.length > 35 ? `${name.substring(0, 35)}...` : name}, ${x.error}</div>`
                        });
                        this.toastrService.error(message, this.translationService.translate('error.failedToScraping'), {
                            extendedTimeOut: 0,
                            closeButton: true,
                            tapToDismiss: false,
                            enableHtml: true,
                            toastClass: 'ngx-toastr ngx-toastr-width'
                        });
                    }
                    else
                        this.toastrService.success(this.translationService.translate('message.checkConnection'));
                }).catch((error: Error) => {
                    throw error;
                }).finally(() => {
                    this.table.refresh();
                });
        } else {
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
        }
    }
    importTraderBoardMember() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {
            this.traderBoardMemberUow.importTo(keys, this.chatHubService?.connectionId)
                .then(() => {
                    this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                }).catch((error: Error) => {
                    throw error;
                }).finally(() => {
                    window.open(`${this.baseUrl}docs/customer-activity-log`, "_blank");
                });
        } else {
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
        }
    }

    importKadTaxisNet() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {
            this.traderKadUow.importTo(keys, this.chatHubService?.connectionId)
                .then(() => {
                    this.toastrService.success(this.translationService.translate('message.processCompleted'));
                }).catch((error: Error) => {
                    throw error;
                });
        } else {
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
        }
    }

    importKadBranchesTaxisNet() {
        const keys = this.table.GetSelectedKeys();

        if (keys.length > 0) {
            this.traderBranchUow.importTo(keys, this.chatHubService?.connectionId)
                .then(() => {
                    this.toastrService.success(this.translationService.translate('message.processCompleted'));
                }).catch((error: Error) => {
                    throw error;
                });
        } else {
            this.toastrService.info(this.translationService.translate('message.hasSelections'));
        }
    }
}
