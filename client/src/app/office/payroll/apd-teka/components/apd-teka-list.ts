import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { MenuItem } from "@progress/kendo-angular-menu";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { ToastrService } from "ngx-toastr";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { lastValueFrom } from 'rxjs';

import { DialogResult, ApdTekaUnitOfWork, GridViewToken, IFormlyFormInputs, FormlyEditDialogComponent } from "@officeNg";
import { TranslationService } from "@core";

@Component({
    selector: "apd-teka-list",
    templateUrl: "./apd-teka-list.html",
})
export class ApdTekaListComponent implements OnInit {
    @ViewChild(GridViewToken) gridView: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];
    pathUrl = 'office/apd-teka';
    menuItems: MenuItem[];
    
    filterInputs: IFormlyFormInputs;
    dialogFormModel: any;

    constructor(
        @Inject('BASE_URL') private baseUrl: string,
        public uow: ApdTekaUnitOfWork,
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
    }

    selectedMenuItem(item: MenuItem) {

        switch (item.data) {
            case '1':
                this.createPeriod();
                break;

            case '2':
                this.payrollStatus();
                break;

            case '3':
                this.apdSubmit();
                break;

            case '4':
                this.tekaSubmit();
                break;
        }
    }

    ngOnInit(): void {
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
                        text: this.translationService.translate('menu.createPeriod')
                    }, {
                        data: "2",
                        text: this.translationService.translate('menu.payrollStatus')
                    }, {
                        data: "3",
                        text: this.translationService.translate('menu.apdSubmit')
                    }, {
                        data: "4",
                        text: this.translationService.translate('menu.tekaSubmit')
                    }
                ]
            }
        ];

        this.selectedKeysChange([]);

        this.uow.selectCompanyPeriodDialog()
            .then((formModel: any) => {
                this.dialogFormModel = formModel;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
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

    createPeriod() {
        const dialogRef: DialogRef = this.dialogService.open({
            content: FormlyEditDialogComponent,
            minHeight: 200, minWidth: 250, width: 350, actionsLayout: 'end'
        });

        const instance = dialogRef.content.instance as FormlyEditDialogComponent;
        const data = {
            model: {
                companyId: 0,
                period: new Date()
            },
            formModel: this.dialogFormModel
        };
        instance.createLabel = true;
        instance.setModel(data);

        dialogRef.result
            .subscribe((result: any) => {

                if (result?.text === "Submit") {
                    const period = instance.model.period as Date;
                    this.uow.createPeriod(period.getFullYear(), period.getMonth() + 1)
                        .then(() => {
                            setTimeout(() => {
                                this.gridView.refresh().then(() => {
                                    this.gridView.ClearSelectedKeys();
                                });
                            }, 100);
                        })
                        .catch((err: Error) => {
                            throw err;
                        }).finally(() => {
                            this.toastrService.success(this.translationService.translate('message.processCompleted'));
                        });
                }
            });
    }

    payrollStatus() {
        const selectedKeys = this.gridView.GetSelectedKeys();

        this.uow.payrollStatus(selectedKeys)
            .then(() => {
                setTimeout(() => {
                    this.gridView.refresh().then(() => {
                        this.gridView.ClearSelectedKeys();
                    });
                }, 100);
            })
            .catch((err: Error) => {
                throw err;
            }).finally(() => {
                this.toastrService.success(this.translationService.translate('message.processCompleted'));
            });
    }

    apdSubmit() {
        const selectedKeys = this.gridView.GetSelectedKeys();

        this.uow.apdSubmit(selectedKeys)
            .then(() => {
                setTimeout(() => {
                    this.gridView.refresh().then(() => {
                        this.gridView.ClearSelectedKeys();
                    });
                }, 100);
            })
            .catch((err: Error) => {
                throw err;
            }).finally(() => {
                this.toastrService.success(this.translationService.translate('message.processCompleted'));

                //setTimeout(() => {
                //    window.open(`${this.baseUrl}docs/customer-activity-log`, "_blank");
                //}, 500);
            });
    }

    tekaSubmit() {
        const selectedKeys = this.gridView.GetSelectedKeys();

        this.uow.tekaSubmit(selectedKeys)
            .then(() => {
                setTimeout(() => {
                    this.gridView.refresh().then(() => {
                        this.gridView.ClearSelectedKeys();
                    });
                }, 100);
            })
            .catch((err: Error) => {
                throw err;
            }).finally(() => {
                this.toastrService.success(this.translationService.translate('message.processCompleted'));

                //setTimeout(() => {
                //    window.open(`${this.baseUrl}docs/customer-activity-log`, "_blank");
                //}, 500);
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
        const disabled = !(keys.length > 0);
        this.menuItems[0].items[1].disabled = disabled;
        this.menuItems[0].items[2].disabled = disabled;
        this.menuItems[0].items[3].disabled = disabled;
    }
}
