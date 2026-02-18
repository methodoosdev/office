import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { MenuItem } from "@progress/kendo-angular-menu";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { ToastrService } from "ngx-toastr";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { lastValueFrom, Subscription } from 'rxjs';

import { DialogResult, EmailMessageUnitOfWork, GridViewToken, IFormlyFormInputs } from "@officeNg";
import { ChatHubService, TranslationService } from "@core";

@Component({
    selector: "email-message-list",
    templateUrl: "./email-message-list.html",
    providers: [ChatHubService]
})
export class EmailMessageListComponent implements OnInit, OnDestroy {
    @ViewChild(GridViewToken) gridView: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];
    pathUrl = 'office/email-message';
    menuItems: MenuItem[];
    
    //private subscription: Subscription;

    filterInputs: IFormlyFormInputs;

    constructor(
        public uow: EmailMessageUnitOfWork,
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private chatHubService: ChatHubService,
        private translationService: TranslationService) {
    }

    ngOnDestroy(): void {
        //this.subscription.unsubscribe();
        //this.chatHubService.stop();
    }

    selectedMenuItem(item: MenuItem) {

        switch (item.data) {
            case '1':
                this.showConfirmation(() => this.sendFinancialObligationEmails());
                break;

            case '2':
                this.showConfirmation(() => this.sendSelectedEmails());
                break;
        }
    }

    ngOnInit(): void {
        //this.chatHubService.start();

        //this.subscription = this.chatHubService.progress$.subscribe((value: number) => {
        //    //this.requestInputs.properties["progress"].props["value"] = value;
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
                        text: this.translationService.translate('menu.sendFinancialObligationEmails')
                    }, {
                        data: "2",
                        disabled: true,
                        text: this.translationService.translate('menu.sendSelectedEmails')
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

        const dialog: DialogRef = this.dialogService.open({
            title: this.translationService.translate('common.confirmation'),
            content: this.translationService.translate('common.areYouSure'),
            actions: actions,
            width: 450, height: 200, minWidth: 250, actionsLayout: 'end'
        });

        lastValueFrom(dialog.result)
            .then((result: any) => {
                if (result['dialogResult'] === DialogResult.Ok) {
                    callback();
                }
            });
    }

    sendFinancialObligationEmails() {
        this.uow.sendFinancialObligationEmails(this.chatHubService?.connectionId)
            .then(() => {
                this.gridView.refresh().then(() => {
                    this.toastrService.success(this.translationService.translate('message.processCompleted'));
                });

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    sendSelectedEmails() {
        this.uow.sendSelectedEmails(this.gridView.GetSelectedKeys(), this.chatHubService?.connectionId)
            .then(() => {
                this.gridView.refresh().then(() => {
                    this.toastrService.success(this.translationService.translate('message.processCompleted'));
                });

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
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
