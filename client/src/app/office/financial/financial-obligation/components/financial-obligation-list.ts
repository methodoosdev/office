import { Component, EventEmitter, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Router, NavigationEnd } from "@angular/router";
import { Location as Location2 } from "@angular/common";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { MenuItem } from "@progress/kendo-angular-menu";

import { DialogResult, FinancialObligationUnitOfWork, GridViewToken, IFormlyFormInputs } from "@officeNg";
import { ProgressHubService, TranslationService } from "@core";
import { lastValueFrom, filter, Subscription } from "rxjs";

export interface IFinancialObligationRequestModel {
    traderIds: number[];
    personIds: number[];
    financialObligationIds: number[];
    retrieve: string;
}

@Component({
    selector: "financial-obligation-list",
    templateUrl: "./financial-obligation-list.html",
    providers: [ProgressHubService]
})
export class FinancialObligationListComponent implements OnInit, OnDestroy {
    @ViewChild(GridViewToken) gridView: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];
    menuItems: MenuItem[];
    pathUrl = 'office/financial-obligation';
    dialog: DialogRef;
    
    private dialogCloseSub: Subscription;
    private progressChange: EventEmitter<string> = new EventEmitter();

    requestInputs: IFormlyFormInputs;
    filterInputs: IFormlyFormInputs;

    constructor(location: Location2, 
        public uow: FinancialObligationUnitOfWork,
        private router: Router,
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private hubService: ProgressHubService,
        private translationService: TranslationService) {

        this.dialogCloseSub = router.events.pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(() => {
                const path = location.path();
                if (path != "/financial-obligation") {
                    this.dialog?.close();
                }
            });
    }

    selectedMenuItem(item: MenuItem) {

        switch (item.data) {
            case '1':
                this.showConfirmation(() => this.createEmailMessages());
                break;

            case '2':
                this.showConfirmation(() => this.createSelectedEmailMessages());
                break;

            case '3':
                this.router.navigateByUrl("/office/email-message");
                break;
        }
    }

    ngOnDestroy(): void {
        this.dialogCloseSub?.unsubscribe();
        //this.hubService.stop();
    }

    ngOnInit(): void {
        //this.hubService.start();
        //this.hubService.hubConnection.on('progressLabel', (message: string) => {
        //    this.progressChange.emit(message);
        //});

        this.settingsMenuItems = [
            {
                text: this.translationService.translate('common.saveState'),
                click: (): void => {
                    this.gridView.saveState();
                }
            }, {
                text: this.translationService.translate('common.removeState'),
                click: (): void => {
                    this.gridView.removeState();
                }
            }];

        this.menuItems = [
            {
                data: "0",
                text: this.translationService.translate('common.actions'),
                items: [
                    {
                        data: "1",
                        text: this.translationService.translate('menu.createEmailMessages')
                    }, {
                        data: "2",
                        disabled: true,
                        text: this.translationService.translate('menu.createSelectedEmailMessages')
                    }, {
                        separator: true
                    }, {
                        data: "3",
                        text: this.translationService.translate('menu.emailMessage')
                    }
                ]
            }
        ];
    }

    showConfirmation(callback: any): void {
        const actions = [
            { text: this.translationService.translate('common.yes'), themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: this.translationService.translate('common.cancel'), dialogResult: DialogResult.Cancel }
        ];

        this.dialog = this.dialogService.open({
            title: this.translationService.translate('common.confirmation'),
            content: this.translationService.translate('common.areYouSure'),
            actions: actions,
            width: 450, height: 200, minWidth: 250, actionsLayout: 'end'
        });

        lastValueFrom(this.dialog.result)
            .then((result: any) => {
                if (result['dialogResult'] === DialogResult.Ok) {
                    callback();
                }
            });
    }

    createEmailMessages() {
        this.uow.createEmailMessages(this.hubService?.connectionId)
            .then(() => {
                this.gridView.refresh().then(() => {
                    this.toastrService.success(this.translationService.translate('message.processCompleted'));
                });

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    createSelectedEmailMessages() {
        this.uow.createSelectedEmailMessages(this.gridView.GetSelectedKeys(), this.hubService?.connectionId)
            .then(() => {
                this.gridView.refresh().then(() => {
                    this.gridView.ClearSelectedKeys();
                    this.toastrService.success(this.translationService.translate('message.processCompleted'));
                });

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    requestInputsChange(inputs: IFormlyFormInputs) {
        inputs.properties['traderIds'].props.options = inputs.customProperties['traderOptions1'];
        inputs.properties['serviceIds'].props.options = inputs.customProperties['serviceOptions1'];
        inputs.model['serviceIds'] = inputs.customProperties['serviceIds1'];

        inputs.properties['retrieve'].props['click'] = (field: FormlyFieldConfig, event: any) => {

            field.props.disabled = true;
            this.uow.retrieve(inputs.model, this.hubService?.connectionId)
                .then(() => {
                    this.gridView.refresh().then(() => {
                        inputs.form.get("progress").setValue("");
                        this.toastrService.success(this.translationService.translate('message.processCompleted'));
                    });
                }).catch((e: any) => {
                    this.toastrService.error(e.error);
                }).finally(() => field.props.disabled = false);
        };

        inputs.properties['choiceId'].props['change'] = (field: FormlyFieldConfig, event: any) => {
            if (field.model['choiceId'] == 1) {
                inputs.properties['traderIds'].props.options = inputs.customProperties['traderOptions1'];
                inputs.properties['serviceIds'].props.options = inputs.customProperties['serviceOptions1'];
                inputs.form.get('traderIds').reset([]);
                inputs.form.get('serviceIds').reset(inputs.customProperties['serviceIds1']);
            } else {
                inputs.properties['traderIds'].props.options = inputs.customProperties['traderOptions2'];
                inputs.properties['serviceIds'].props.options = inputs.customProperties['serviceOptions2'];
                inputs.form.get('traderIds').reset([]);
                inputs.form.get('serviceIds').reset(inputs.customProperties['serviceIds2']);
            }
        };

        this.progressChange.subscribe((value: string) => {
            inputs.form.get("progress").setValue(value);
        });

        this.requestInputs = inputs;
    }

    filterInputsChange(filterInputs: IFormlyFormInputs) {

        filterInputs.properties['saveState']['expressions'] = {
            'props.disabled': (field: FormlyFieldConfig) => {
                return !field.form.dirty;
            }
        };

        filterInputs.properties['saveState'].props['click'] = () => {
            this.gridView.filterSaveState(filterInputs);
        };

        filterInputs.properties['removeState'].props['click'] = () => {
            this.gridView.filterRemoveState(filterInputs);
        };

        this.filterInputs = filterInputs
    }

    selectedKeysChange(keys: any[]) {
        this.menuItems[0].items[1].disabled = !(keys.length > 0);
    }
}
