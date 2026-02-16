import { Component, EventEmitter, Input, Output } from '@angular/core';

import { ToastrService } from 'ngx-toastr';
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';

import { DialogResult, ColumnButtonClickEvent, FormListBaseComponent } from '../../api/public-api';
import { PrimeNGConfig } from '@primeNg';
import { UnitOfWork } from "../../api/unit-of-work";
import { FormListDialogComponent } from '../form-list-dialog/form-list-dialog.component';
import { lastValueFrom } from 'rxjs';
import { GroupableSettings } from '@progress/kendo-angular-grid';

@Component({
    selector: 'form-list-detail-mapping',
    templateUrl: 'form-list-detail-mapping.component.html'
})
export class FormListDetailMappingComponent extends FormListBaseComponent {
    @Input() checkboxColumnVisible: boolean = true;
    @Input() showSelectAll: boolean = true;
    @Input() dialogShowSelectAll: boolean = true;
    @Input() insertButtonVisible: boolean = true;
    @Input() deleteButtonVisible: boolean = true;
    @Input() copyButtonVisible: boolean = false;
    @Input() dialogTitle: string;
    @Input() dialogUow: UnitOfWork;
    @Input() groupable: boolean | GroupableSettings = false;

    copyLabel: string;
    insertLabel: string;
    deleteLabel: string;
    searchLabel: string;
    autofitColumnsLabel: string;
    settingsMenuLabel: string;
    yesLabel: string;
    noLabel: string;

    //events
    @Output() onColumnButtonClick: EventEmitter<ColumnButtonClickEvent> = new EventEmitter();
    @Output() onCopyFunctionClick: EventEmitter<number> = new EventEmitter();

    constructor(
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private config: PrimeNGConfig) {
        super();

        this.yesLabel = this.config.getTranslation('common.yes');
        this.noLabel = this.config.getTranslation('common.no');
        this.copyLabel = this.config.getTranslation('common.copy');
        this.insertLabel = this.config.getTranslation('common.insert');
        this.deleteLabel = this.config.getTranslation('common.delete');
        this.searchLabel = this.config.getTranslation('common.search');
        this.autofitColumnsLabel = this.config.getTranslation('common.autofit');
    }

    onColumnButtonClickEvent(event: Event, action: string, dataItem: any) {
        event.stopPropagation();
        this.onColumnButtonClick.emit({ action, dataItem });
    }

    insert() {
        const dialogRef = this.dialogService.open({
            content: FormListDialogComponent,
            width: 1120,
            height: 570
        });

        const component = dialogRef.content.instance as FormListDialogComponent;
        component.title = this.dialogTitle;
        component.uow = this.dialogUow;
        component.showSelectAll = this.dialogShowSelectAll;

        dialogRef.result.subscribe((ids) => {
            if (Array.isArray(ids)) {

                this.uow.importMapping(ids, this.parentId)
                    .then(() => {
                        Promise.resolve(null).then(() => {
                            this._loadDataSourceCore();
                        }).then(() => {
                            this.toastrService.success(this.config.getTranslation('message.insertionCompleted'));
                        });
                    }).catch((error: Error) => {
                        throw error;
                    });
            }
        });
    }

    delete() {
        const actions = [
            { text: this.config.getTranslation('common.yes'), themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: this.config.getTranslation('common.cancel'), dialogResult: DialogResult.Cancel }
        ];

        const dialog: DialogRef = this.dialogService.open({
            title: this.config.getTranslation('common.confirmation'),
            content: this.config.getTranslationWithParams('message.deleteSelection', this.selectedKeys.length),
            actions: actions,
            width: 450, height: 200, minWidth: 250, actionsLayout: 'end'
        });

        lastValueFrom(dialog.result)
            .then((result: any) => {
                if (result['dialogResult'] === DialogResult.Ok) {
                    this.uow.removeMapping(this.selectedKeys, this.parentId)
                        .then(() => {
                            this.selectedKeys = [];
                            Promise.resolve(null).then(() => {
                                this._loadDataSourceCore();
                            }).then(() => {
                                this.toastrService.success(this.config.getTranslation('message.deletionCompleted'));
                            });
                        }).catch((err: Error) => {
                            throw err;
                        });
                }
            });
    }

    copyFunction() {
        this.onCopyFunctionClick.emit(this.parentId);
    }
}
