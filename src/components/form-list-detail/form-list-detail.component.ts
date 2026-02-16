import { Component, EventEmitter, Input, Output } from '@angular/core';

import { ToastrService } from 'ngx-toastr';
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { CellClickEvent, GroupableSettings, RowClassFn, SelectableSettings, SortSettings } from '@progress/kendo-angular-grid';

import { PrimeNGConfig } from '@primeNg';
import { DialogResult, ColumnButtonClickEvent, FormListBaseComponent } from '../../api/public-api';
import { lastValueFrom } from 'rxjs';

@Component({
    selector: 'form-list-detail',
    templateUrl: 'form-list-detail.component.html'
})
export class FormListDetailComponent extends FormListBaseComponent {
    @Input() allowDblClickRow = false;
    @Input() toExcelButtonVisible = false;
    @Input() toPdfButtonVisible = false;
    @Input() excelFileName: string = 'excelFileName';
    @Input() pdfFileName: string = 'pdfFileName';
    @Input() checkboxColumnVisible: boolean = true;
    @Input() showSelectAll: boolean = true;
    @Input() insertButtonVisible: boolean = true;
    @Input() deleteButtonVisible: boolean = true;
    @Input() pathUrl: string;
    @Input() groupable: boolean | GroupableSettings = false;

    @Input() selectableSettings: SelectableSettings = {
        enabled: true,
        checkboxOnly: false,
        mode: `multiple`,
        cell: false,
        drag: false
    };
    @Input() sortable: SortSettings = { allowUnsort: false, mode: 'single' };
    @Input() rowClass: RowClassFn = () => { return ""; };

    insertLabel: string;
    deleteLabel: string;
    searchLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    //events
    @Output() onColumnButtonClick: EventEmitter<ColumnButtonClickEvent> = new EventEmitter();
    @Output() onInsertButtonClick: EventEmitter<any> = new EventEmitter();
    @Output() onDblRowClick: EventEmitter<any> = new EventEmitter();

    clickedRowItem: any;

    constructor(
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private config: PrimeNGConfig) {
        super();

        this.yesLabel = this.config.getTranslation('common.yes');
        this.noLabel = this.config.getTranslation('common.no');
        this.insertLabel = this.config.getTranslation('common.insert');
        this.deleteLabel = this.config.getTranslation('common.delete');
        this.searchLabel = this.config.getTranslation('common.search');
        this.autofitColumnsLabel = this.config.getTranslation('common.autofit');
    }

    onCellClick(event: CellClickEvent) {
        if (event.type == 'click')
            this.clickedRowItem = {
                dataItem: event.dataItem,
                column: event.column
            };
    }

    onDblClick(event: Event) {
        event.preventDefault(); // do not remove it
        if (this.allowDblClickRow && this.clickedRowItem) {
            this.onInsertButtonClick.emit({
                id: this.clickedRowItem["dataItem"][this.searchModel.dataKey],
                parentId: this.parentId
            });
        }
    }

    onColumnButtonClickEvent(event: Event, action: string, dataItem: any) {
        event.stopPropagation();
        this.onColumnButtonClick.emit({ action, dataItem });
    }

    insert() {
        this.onInsertButtonClick.emit({
            id: 0,
            parentId: this.parentId
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
                    this.uow.deleteSelected(this.selectedKeys)
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

    refresh() {
        this._loadDataSourceCore();
    }
}
