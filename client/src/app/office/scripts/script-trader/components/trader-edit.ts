import { Component, QueryList, ViewChild, ViewChildren, ViewEncapsulation, signal } from "@angular/core";
import {
    ScriptTraderUnitOfWork, FormlyEditDialogComponent, DialogResult,
    ScriptTableUnitOfWork, ScriptTableItemUnitOfWork, ScriptFieldUnitOfWork, ScriptUnitOfWork, ScriptItemUnitOfWork,
    ScriptGroupUnitOfWork, ScriptToolUnitOfWork, ScriptToolItemUnitOfWork,
    FormEditToken, AfterModelChangeEvent, FormListDetailDialogComponent, UnitOfWork, ColumnButtonClickEvent
} from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";
import { Observable } from "rxjs";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { DataResult, GroupResult } from "@progress/kendo-data-query";
import { saveAs } from "@progress/kendo-file-saver";
import { firstValueFrom, take } from "rxjs";

const isGroupResult = (v: unknown): v is GroupResult =>
    !!v &&
    typeof (v as any).field === 'string' &&
    Array.isArray((v as any).items); // (optional) && typeof (v as any).aggregates === 'object'

export const isGrouped = (dr: DataResult): dr is { data: GroupResult[]; total: number } =>
    Array.isArray(dr.data) && dr.data.length > 0 && isGroupResult(dr.data[0]);

@Component({
    selector: "script-trader-edit",
    templateUrl: "./trader-edit.html",
    encapsulation: ViewEncapsulation.None,
    host: {
        class: 'script-trader-edit'
    },
    styles: [
        `
            .script-trader-edit .k-expander-title {
                color: #ff5722
            }
        `
    ]
})

export class ScriptTraderEditComponent implements CanComponentDeactivate {
    @ViewChild("scriptGroupGrid") scriptGroupGrid: FormListDetailDialogComponent;
    @ViewChild("scriptTableGrid") scriptTableGrid: FormListDetailDialogComponent;
    @ViewChildren("scriptTableItemGrid") scriptTableItemGridList: QueryList<FormListDetailDialogComponent>;
    @ViewChild("scriptFieldGrid") scriptFieldGrid: FormListDetailDialogComponent;
    @ViewChild("scriptGrid") scriptGrid: FormListDetailDialogComponent;
    @ViewChildren("scriptItemGrid") scriptItemGridList: QueryList<FormListDetailDialogComponent>;
    @ViewChild("scriptToolGrid") scriptToolGrid: FormListDetailDialogComponent;
    @ViewChildren("scriptToolItemGrid") scriptToolItemGridList: QueryList<FormListDetailDialogComponent>;
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/script-trader';
    parentId: number;
    categoryBookTypeId: number;
    scriptItemSelectedKeys: any[] = [];
    groupHeaderInputs: string[] = [];

    scriptToolsLabel: string;
    scriptGroupsLabel: string;
    scriptsLabel: string;
    scriptTablesLabel: string;
    scriptFieldsLabel: string;
    printLabel: string;

    configForm = new FormGroup({});
    configOptions: FormlyFormOptions = {};
    configFields: FormlyFieldConfig[];
    configModel: any;

    uploadSaveUrl: string = "/api/scriptTool/upload";
    dataItemId = signal(0);

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public uow: ScriptTraderUnitOfWork,
        public scriptGroupUow: ScriptGroupUnitOfWork,
        public scriptTableUow: ScriptTableUnitOfWork,
        public scriptTableItemUow: ScriptTableItemUnitOfWork,
        public scriptFieldUow: ScriptFieldUnitOfWork,
        public scriptUow: ScriptUnitOfWork,
        public scriptItemUow: ScriptItemUnitOfWork,
        public scriptToolUow: ScriptToolUnitOfWork,
        public scriptToolItemUow: ScriptToolItemUnitOfWork,
        public dialogService: DialogService,
        private toastrService: ToastrService,
        private translationService: TranslationService
    ) { }

    ngOnInit() {
        this.scriptToolsLabel = this.translationService.translate('menu.scriptTools');
        this.scriptGroupsLabel = this.translationService.translate('menu.scriptGroups');
        this.scriptsLabel = this.translationService.translate('menu.scripts');
        this.scriptTablesLabel = this.translationService.translate('menu.scriptTables');
        this.scriptFieldsLabel = this.translationService.translate('menu.scriptFields');
        this.printLabel = this.translationService.translate("common.print")

        const traderId = +this.route.snapshot.paramMap.get('id');

        this.scriptToolUow.getConfig(traderId)
            .then((data: any) => {
                this.configFields = data.configModel.customProperties.fields;
                this.configModel = data.configModel;

                //const fieldProperties = getFieldProperties(this.searchFields);
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    //async onFileChange(files: File[]) {
    //    await this.exportToExcelService.fileChange(files);
    //}

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return true;
    }

    afterModelChange(e: AfterModelChangeEvent) {
        this.parentId = e.model.id;
        this.categoryBookTypeId = e.model.categoryBookTypeId;

        e.fieldProperties['cloneScripts'].props['click'] = () => {
            this.showConfirmationClone(e.model.taxSystemId, e.model.id);
        };

        e.fieldProperties['deleteScripts'].props['click'] = () => {
            this.showConfirmationDelete(e.model.id);
        };

    }

    private showConfirmationDelete(targetTraderId: number): void {
        const actions = [
            { text: this.translationService.translate('common.yes'), themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: this.translationService.translate('common.cancel'), dialogResult: DialogResult.Cancel }
        ];

        const dialog = this.dialogService.open({
            title: this.translationService.translate('common.confirmation'),
            content: this.translationService.translate('message.deleteConfirmation'),
            actions: actions,
            width: 450, height: 200, minWidth: 250, actionsLayout: 'end'
        });

        firstValueFrom(dialog.result)
            .then((result: any) => {
                if (result['dialogResult'] === DialogResult.Ok) {

                    this.uow.deleteScripts(targetTraderId)
                        .then((result: any) => {
                            const url = this.router.url;
                            this.router.navigateByUrl('/office', { skipLocationChange: true }).then(() => {
                                this.router.navigate([url]).then(() => {
                                    this.toastrService.success(this.translationService.translate('message.successfullyCompleted'));
                                });
                            });

                        }).catch((e: any) => {
                            this.toastrService.error(e.error);
                        });
                }
            });
    }
    private showConfirmationClone(sourceTraderId: number, targetTraderId: number): void {
        const actions = [
            { text: this.translationService.translate('common.yes'), themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: this.translationService.translate('common.cancel'), dialogResult: DialogResult.Cancel }
        ];

        const dialog = this.dialogService.open({
            title: this.translationService.translate('common.clone'),
            content: this.translationService.translate('message.cloneScripts'),
            actions: actions,
            width: 450, height: 200, minWidth: 250, actionsLayout: 'end'
        });

        firstValueFrom(dialog.result)
            .then((result: any) => {
                if (result['dialogResult'] === DialogResult.Ok) {

                    this.uow.cloneScripts(sourceTraderId, targetTraderId)
                        .then((result: any) => {
                            const url = this.router.url;
                            this.router.navigateByUrl('/office', { skipLocationChange: true }).then(() => {
                                this.router.navigate([url]).then(() => {
                                    this.toastrService.success(this.translationService.translate('message.successfullyCompleted'));
                                });
                            });

                        }).catch((e: any) => {
                            this.toastrService.error(e.error);
                        });
                }
            });
    }

    groupHeaderInputsChange(value: string[]) {
        this.groupHeaderInputs = value;
    }

    onUploadSuccess(e: any) {
        this.scriptToolGrid.refresh();
    }

    scriptToolGridButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'loadPrototype') {
            this.dataItemId.set(event.dataItem.id);
        }
        if (event.action === 'downloadExcel') {
            if (!event.dataItem.fileName) {
                this.toastrService.info(this.translationService.translate('message.UploadToExportExcel'));
                return;
            }

            this.scriptToolUow.downloadExcel(event.dataItem.id, this.configModel)
                .then((result: any) => {
                    saveAs(result, "Scenario.xlsx");
                }).catch((e: any) => {
                    this.toastrService.error(e.error);
                });
        }
        if (event.action === 'downloadPrototype') {
            if (!event.dataItem.fileName) {
                this.toastrService.info(this.translationService.translate('message.UploadToExportExcel'));
                return;
            }

            this.scriptToolUow.downloadPrototype(event.dataItem.id)
                .then((result: any) => {
                    saveAs(result, event.dataItem.fileName);
                }).catch((e: any) => {
                    this.toastrService.error(e.error);
                });
        }
        if (event.action === 'tool') {

            const json = {
                active: this.configModel.active,
                fiscalYear: this.configModel.fiscalYear,
                periodFrom: this.configModel.periodFrom,
                periodTo: this.configModel.periodTo,
                inventory: this.configModel.inventory,
            };

            const url = this.router.serializeUrl(
                this.router.createUrlTree(
                    ['/office/script-tool', event.dataItem.id, this.parentId, JSON.stringify(json)],
                    { relativeTo: null }
                )
            );
            window.open(window.location.origin + url, '_blank');
        }
        if (event.action === 'diagram') {

            const url = this.router.serializeUrl(
                this.router.createUrlTree(
                    ['/office/script-diagram', event.dataItem.id, this.parentId],
                    { relativeTo: null }
                )
            );
            window.open(window.location.origin + url, '_blank');
        }
    }

    printCommand() {
        const json = {
            active: this.configModel.active,
            fiscalYear: this.configModel.fiscalYear,
            periodFrom: this.configModel.periodFrom,
            periodTo: this.configModel.periodTo,
            inventory: this.configModel.inventory,
        };

        const url = this.router.serializeUrl(
            this.router.createUrlTree(
                ['/office/script-report', this.parentId, this.categoryBookTypeId, JSON.stringify(json)],
                { queryParams: { groups: this.groupHeaderInputs }, relativeTo: null }
            )
        );
        window.open(window.location.origin + url, '_blank');
    }

    //scriptTableItemsShowIf = (item: any): boolean => {
    //    return item.hasChildren > 0;
    //}

    public getProperties(id: number, parentId: number, uow: UnitOfWork) {
        return uow.getEntity(id, parentId, null)
            .catch((err: Error) => {
                Promise.reject(err);
            });
    }

    onScriptGroupChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptGroupUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 450, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);

                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptGroupUow.commit(instance.model)
                                .then(() => {
                                    this.scriptGroupGrid.refresh();
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

    onScriptTableChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptTableUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 450, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);

                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptTableUow.commit(instance.model)
                                .then(() => {
                                    this.scriptTableGrid.refresh();
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

    onScriptTableItemChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptTableItemUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 650, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);

                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptTableItemUow.commit(instance.model)
                                .then(() => {
                                    this.scriptTableItemGridList.toArray().map(x => x.refresh());
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

    onScriptFieldChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptFieldUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 650, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);
                instance.fieldProperties['fieldName'].props['onClick'] = (field: FormlyFieldConfig, trader: any) => {

                    const keys = ['scriptTableId', 'scriptQueryTypeId', 'scriptFunctionTypeId'] as const;
                    const key = keys[field.form.get('scriptFieldTypeId')?.value as number];

                    key && field.form.get('fieldName')?.setValue(
                        ((instance.fieldProperties[key]?.props?.options ?? []) as { value: any; label: string }[])
                            .find(o => o.value === field.form.get(key)?.value)?.label ?? null
                    );
                    //const scriptFieldTypeId: number = field.form.get('scriptFieldTypeId').value;

                    //switch (scriptFieldTypeId) {
                    //    case 0:
                    //        const scriptTableId = field.form.get('scriptTableId').value;
                    //        const items1 = instance.fieldProperties['scriptTableId'].props['options'] as any[];
                    //        const label1 = items1.find(x => x.value === scriptTableId)?.label ?? null;
                    //        field.form.get('fieldName').setValue(label1);
                    //        break;

                    //    case 1:
                    //        const scriptQueryTypeId = field.form.get('scriptQueryTypeId').value;
                    //        const items2 = instance.fieldProperties['scriptQueryTypeId'].props['options'] as any[];
                    //        const label2 = items2.find(x => x.value === scriptQueryTypeId)?.label ?? null;
                    //        field.form.get('fieldName').setValue(label2);
                    //        break;

                    //    case 2:
                    //        const scriptFunctionTypeId = field.form.get('scriptFunctionTypeId').value;
                    //        const items3 = instance.fieldProperties['scriptFunctionTypeId'].props['options'] as any[];
                    //        const label3 = items3.find(x => x.value === scriptFunctionTypeId)?.label ?? null;
                    //        field.form.get('fieldName').setValue(label3);
                    //        break;

                    //    default:
                    //        break;
                    //}

                    //scriptFieldTypeId scriptQueryTypeId scriptTableId scriptFunctionTypeId

                    //0 { value: 0, label: "Πίνακας", disabled: false }
                    //1 { value: 1, label: "Ερώτημα στη βάση", disabled: false }
                    //2 { value: 2, label: "Διαδικασία", disabled: false }
                    //3 { value: 3, label: "Σταθερή τιμή", disabled: false }

                    //const scriptName: string = field.form.get('scriptName').value;
                    //const value = `#${scriptName.trim().replace(/\s+/g, ".")}`;
                    //field.form.get('fieldName').setValue(value);
                };

                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptFieldUow.commit(instance.model)
                                .then(() => {
                                    this.scriptFieldGrid.refresh();
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

    onScriptChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 650, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);
                instance.fieldProperties['replacement'].props['onClick'] = (field: FormlyFieldConfig, trader: any) => {

                    const scriptName: string = field.form.get('scriptName').value;
                    const value = `#${scriptName.trim().replace(/\s+/g, ".")}`;
                    field.form.get('replacement').setValue(value);
                };


                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptUow.commit(instance.model)
                                .then(() => {
                                    this.scriptGrid.refresh();
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

    onScriptItemChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptItemUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 650, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);

                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptItemUow.commit(instance.model)
                                .then(() => {
                                    this.scriptItemGridList.toArray().map(x => x.refresh());
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

    onScriptToolChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptToolUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 450, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);

                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptToolUow.commit(instance.model)
                                .then(() => {
                                    this.scriptToolGrid.refresh();
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

    onScriptToolItemChange(model: any) {

        this.getProperties(model.id, model.parentId, this.scriptToolItemUow)
            .then((data) => {
                const dialogRef: DialogRef = this.dialogService.open({
                    content: FormlyEditDialogComponent,
                    minHeight: 200, minWidth: 250, width: 650, actionsLayout: 'end'
                });

                const instance = dialogRef.content.instance as FormlyEditDialogComponent;
                instance.setModel(data);

                dialogRef.result
                    .subscribe((result: any) => {

                        if (result?.text === "Submit") {
                            this.scriptToolItemUow.commit(instance.model)
                                .then(() => {
                                    this.scriptToolItemGridList.toArray().map(x => x.refresh());
                                })
                                .catch((err: Error) => {
                                    throw err;
                                });
                        }
                    });
            });
    }

}
