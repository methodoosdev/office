import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';

import { ToastrService } from 'ngx-toastr';
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { CellClickEvent, DetailTemplateShowIfFn, GroupKey, GroupableSettings, RowClassFn, SelectableSettings, SortSettings } from '@progress/kendo-angular-grid';

import { PrimeNGConfig } from '@primeNg';
import { DialogResult, ColumnButtonClickEvent, FormListBaseComponent } from '../../api/public-api';
import { lastValueFrom } from 'rxjs';
import { GroupDescriptor } from '@progress/kendo-data-query';
import { ButtonThemeColor } from '@progress/kendo-angular-buttons';

export declare type CustomButtonDisabledFn = boolean | (() => boolean);

@Component({
    selector: 'form-list-detail-dialog',
    templateUrl: 'form-list-detail-dialog.component.html'
})
export class FormListDetailDialogComponent extends FormListBaseComponent {
    @Input() allowDblClickRow = true;
    @Input() toExcelButtonVisible = false;
    @Input() toPdfButtonVisible = false;
    @Input() excelFileName: string = 'excelFileName';
    @Input() pdfFileName: string = 'pdfFileName';
    @Input() checkboxColumnVisible: boolean = true;
    @Input() showSelectAll: boolean = true;
    @Input() insertButtonVisible: boolean = true;
    @Input() deleteButtonVisible: boolean = true;
    @Input() pathUrl: string;
    @Input() groupable: GroupableSettings = { enabled: false, showFooter: false };
    @Input() confirmationDeleteEnable: boolean = false;
    @Input() toggleGroupingButtonVisible: boolean = false;

    @Input() groupHeaderInputVisible = false;

    @Input() groupExpanded: boolean = false;
    @Input() expandedGroupKeys: GroupKey[] = [];

    @Input() customButtonVisible: boolean = false;
    @Input() customButtonTheme: ButtonThemeColor = "base";
    @Input() customButtonLabel: string;
    @Input() customButtonDisabled: CustomButtonDisabledFn = false;

    @Input() selectableSettings: SelectableSettings = {
        enabled: true,
        checkboxOnly: true,
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

    @Input() detailTemplate!: TemplateRef<any>;
    @Input() detailTemplateShowIf: DetailTemplateShowIfFn = () => { return true; };

    //events
    @Output() onColumnButtonClick: EventEmitter<ColumnButtonClickEvent> = new EventEmitter();
    @Output() onModifyClick: EventEmitter<any> = new EventEmitter
    @Output() customButtonClick: EventEmitter<any> = new EventEmitter();

    clickedRowItem: any;

    trackBy = (idx: any, item: any) => item.data.id;

    get isCustomButtonDisabled(): boolean {
        const v = this.customButtonDisabled;
        return typeof v === 'function' ? v() : !!v;
    }

    get getShowSelectAll() {
        return this.showSelectAll && this.gridData?.total > 0;
    }

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

    groupHeaderInputChange(checked: boolean, group: string) {
        if (checked) {
            // add if not already in list
            if (!this.groupHeaderInputs.includes(group)) {
                this.groupHeaderInputs.push(group);
            }
        } else {
            // remove if unchecked
            this.groupHeaderInputs = this.groupHeaderInputs.filter(g => g !== group);
        }
    }

    toggleGrouping() {
        this.groupable.enabled = !this.groupable.enabled;
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
            this.modify(this.clickedRowItem.dataItem);
        }
    }

    onColumnButtonClickEvent(event: Event, action: string, dataItem: any) {
        event.stopPropagation();
        this.onColumnButtonClick.emit({ action, dataItem });
    }

    modify(dataItem: any) {
        this.onModifyClick.emit({
            id: dataItem[this.searchModel.dataKey],
            parentId: this.parentId
        });
    }

    onCustomButtonClick() {
        this.customButtonClick.emit({});
    }

    insert() {
        this.onModifyClick.emit({
            id: 0,
            parentId: this.parentId
        });
    }

    private deleteCore() {
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

    delete() {
        if (!this.confirmationDeleteEnable) {
            this.deleteCore();
            return;
        }

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
                    this.deleteCore();
                }
            });
    }

    refresh() {
        this._loadDataSourceCore();
    }
}
