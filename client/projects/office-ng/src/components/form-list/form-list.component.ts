import { AfterContentInit, Component, ContentChildren, EventEmitter, Input, OnInit, Output, QueryList, TemplateRef, ViewChild, ViewChildren, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup } from '@angular/forms';

import { ToastrService } from 'ngx-toastr';
import { CellClickEvent, ColumnBase, DataStateChangeEvent, GridComponent, GridDataResult, GroupableSettings, PagerSettings, RowClassFn, SelectableSettings, SortSettings } from '@progress/kendo-angular-grid';
import { GroupDescriptor, process, SortDescriptor, State } from '@progress/kendo-data-query';
import { ListItemModel } from '@progress/kendo-angular-buttons';
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { MenuItem } from '@progress/kendo-angular-menu';

import { getFieldProperties } from '@formlyNg';
import { stateAnimation, slideInOutAnimation, PrimeTemplate, PrimeNGConfig } from "@primeNg";
import { UnitOfWork } from "../../api/unit-of-work";
import { PersistStateUnitOfWork } from "../../repositories/persist-state-uow";
import { lastValueFrom } from 'rxjs';

import { DialogResult, GridViewToken, ISearchModel, ColumnButtonClickEvent, IFormlyFormInputs, CellButtonDisabledFn } from '../../api/public-api';

@Component({
    selector: 'form-list',
    templateUrl: './form-list.component.html',
    providers: [{ provide: GridViewToken, useExisting: FormListComponent }, PersistStateUnitOfWork],
    animations: [stateAnimation, slideInOutAnimation],
    encapsulation: ViewEncapsulation.None,
    styles: [
        `
           .grid-wrapper {
                position: relative;
            }
            .k-i-loading {
                position: fixed;
                left: 0px;
                top: 0px;
                width: 100%;
                height: 100%;
                z-index: 999999;
                font-size: 3rem;
                background-color: rgba(255, 255, 255, 0.01);
                color: #ff6757;
            }
            .officeNg-form-list .k-grid .k-grouping-header {
                display: none;
            }
        `
    ]
})
export class FormListComponent extends GridViewToken implements OnInit, AfterContentInit {
    @ViewChild(GridComponent, { static: true }) private table: GridComponent;
    @ViewChildren(ColumnBase) _columns: QueryList<ColumnBase>;
    @Input() gridDetailTemplateEnable: boolean = false;
    @Input() animate: boolean = true;
    @Input() uow: UnitOfWork;
    @Input() allowDblClickRow = true;
    @Input() toExcelButtonVisible = false;
    @Input() toPdfButtonVisible = true;
    @Input() filterButtonVisible = false;
    @Input() refreshButtonVisible = false;
    @Input() checkboxColumnVisible = true;
    @Input() showSelectAll: boolean = true;
    @Input() customMenuItems: ListItemModel[];
    @Input() settingsMenuItems: ListItemModel[];
    @Input() menuItems: MenuItem[];
    @Input() customMenuColorButton: any = 'primary';
    @Input() customMenuLabel: string;
    @Input() multipleDelete: boolean = false;
    @Input() newRecordButtonVisible: boolean = true;
    @Input() deleteRecordButtonVisible: boolean = true;
    @Input() goBackButtonVisible: boolean = false;
    @Input() pathUrl: string;
    @Input() parentUrl: string;
    @Input() excelFileName: string = 'excelFileName.xlsx';
    @Input() pdfFileName: string = 'pdfFileName.pdf';
    @Input() parentId: number;
    @Input() queryParams: string;
    @Input() infoModel: any;
    @Input() groupable: boolean | GroupableSettings = false;

    @Input() rowClass: RowClassFn = () => { return ""; };
    @Input() cellButtonDisabled: CellButtonDisabledFn = (c: any, d: any) => { return ""; };
    
    @Input() loadPropertiesBy: Function = () => this.loadProperties();
    @Input() loadDataBy: Function = (result: any) => this.loadData(result);
    @Input() editEntityBy: Function = (url: string, id: number) => this.editEntity(url, id);
    @Input() createEntityBy: Function = () => this.createEntity();

    @Input() sortable: SortSettings = { allowUnsort: false, mode: 'single' };

    skip: number;
    take: number;
    sort: SortDescriptor[];
    group: GroupDescriptor[];

    @Input() selectableSettings: SelectableSettings | boolean = {
        enabled: true,
        checkboxOnly: false,
        mode: `multiple`,
        cell: false,
        drag: false
    };
    showGridDetailTemplate(dataItem: any): boolean {
        return this.gridDetailTemplateEnable;
    }
    pageable: PagerSettings | boolean;
    gridData: GridDataResult;
    loadingData = false;
    loadingProperties = false;

    yesLabel: string;
    noLabel: string;
    insertLabel: string;
    deleteLabel: string;
    searchLabel: string;
    autofitColumnsLabel: string;
    settingsMenuLabel: string;
    downloadLabel: string;

    searchModel: ISearchModel;

    clickedRowItem: any;

    //events
    @Output() onSelectMenuItem: EventEmitter<MenuItem> = new EventEmitter();
    @Output() onColumnButtonClick: EventEmitter<ColumnButtonClickEvent> = new EventEmitter();
    @Output() onLoadProperties: EventEmitter<any> = new EventEmitter();
    @Output() onExportToPdf: EventEmitter<any> = new EventEmitter();
    @Output() onSelectedKeysChange: EventEmitter<any[]> = new EventEmitter();

    headerTemplate: TemplateRef<any>;
    toolbarTemplate: TemplateRef<any>;
    contentTemplate: TemplateRef<any>;
    gridDetailTemplate: TemplateRef<any>;
    @ContentChildren(PrimeTemplate) templates: QueryList<any>;

    trackBy = (idx: any, item: any) => item['data'];

    @Output() onFilterInputsChange: EventEmitter<IFormlyFormInputs> = new EventEmitter();
    @Output() onRequestInputsChange: EventEmitter<IFormlyFormInputs> = new EventEmitter();

    private _selectedKeys: any[] = [];
    get selectedKeys() {
        return this._selectedKeys;
    }

    set selectedKeys(value: any[]) {
        this._selectedKeys = value;
        this.onSelectedKeysChange.emit(value);
    }

    filterPanelVisible: boolean = false;
    filterExistLabel: string;
    filterExist: boolean;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private persistStateUow: PersistStateUnitOfWork,
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private config: PrimeNGConfig) {
        super();

        //set labels
        this.yesLabel = this.config.getTranslation('common.yes');
        this.noLabel = this.config.getTranslation('common.no');
        this.insertLabel = this.config.getTranslation('common.insert');
        this.deleteLabel = this.config.getTranslation('common.delete');
        this.searchLabel = this.config.getTranslation('common.search');
        this.downloadLabel = this.config.getTranslation('common.download');
        this.autofitColumnsLabel = this.config.getTranslation('common.autofit');
        this.settingsMenuLabel = this.config.getTranslation('common.settings');
        this.filterExistLabel = this.config.getTranslation('common.filterExist');
    }

    public GetSelectedKeys() {
        return this.selectedKeys;
    }

    public ClearSelectedKeys() {
        this.selectedKeys = [];
    }

    ngAfterContentInit() {
        this.templates.forEach((item) => {
            switch (item.getType()) {
                case 'header':
                    this.headerTemplate = item.template;
                    break;

                case 'toolbar':
                    this.toolbarTemplate = item.template;
                    break;

                case 'gridDetail':
                    this.gridDetailTemplate = item.template;
                    break;

                case 'content':
                    this.contentTemplate = item.template;
                    break;

                default:
                    this.contentTemplate = item.template;
                    break;
            }
        });
    }

    ngOnInit(): void {
        this.loadPropertiesBy();
    }

    public loadProperties() {
        this.route.params.forEach((params: any) => {

            this.loadingProperties = true;
            this.parentId = this.parentId || (params ? +params.id : null);

            this.uow.loadProperties()
                .then(result => {

                    this.skip = result.searchModel.start;
                    this.take = result.searchModel.pageSize;
                    this.sort = [{
                        field: result.searchModel.sortField,
                        dir: result.searchModel.sortOrder
                    }];
                    this.group = result.searchModel.group;

                    this.pageable = result.searchModel.pagerSettings ? result.searchModel.pagerSettings : false;

                    this.searchModel = result.searchModel;

                    this.onLoadProperties.emit(result);

                    this.loadDataBy && this.loadDataBy(result);

                }).catch((err: Error) => {
                    throw err;
                }).finally(() => {
                    this.loadingProperties = false;
                });
        });
    }

    public loadData(result: any) {

        this.loadDataSource().then(() => {
            //filterModel
            if (result.filterModel) {
                const { fields, ...customPropertiesRest } = result.filterFormModel.customProperties;
                const properties = getFieldProperties(fields);

                const filterInputs = {
                    form: new FormGroup({}),
                    options: {},
                    fields: fields,
                    model: result.filterModel,
                    default: result.filterDefaultModel,
                    properties: properties,
                    customProperties: customPropertiesRest
                };
                this.onFilterInputsChange.emit(filterInputs);
            }

            //requestModel
            if (result.requestModel) {
                const { fields, ...customPropertiesRest } = result.requestFormModel.customProperties;
                const properties = getFieldProperties(fields);

                const requestInputs = {
                    form: new FormGroup({}),
                    options: {},
                    fields: fields,
                    model: result.requestModel,
                    properties: properties,
                    customProperties: customPropertiesRest
                };
                this.onRequestInputsChange.emit(requestInputs);
            }
        });
    }

    loadDataSource() {
        return this._loadDataSourceCore();
    }

    onDataStateChange(state: DataStateChangeEvent): void {
        this.skip = state.skip;
        this.take = state.take;
        this.sort = state.sort;
        this.searchModel['sortField'] = state.sort[0].field;
        this.searchModel['sortOrder'] = state.sort[0].dir;
        this.searchModel['start'] = state.skip;
        this.searchModel['length'] = state.take;

        if (this.group.length > 0) {
            this.searchModel['start'] = 0;
            this.searchModel['length'] = 1000000;
        }

        this.loadDataSource();
    }

    get canExport() {
        return this.gridData && this.gridData.data && this.gridData.data.length > 0;
    }

    refresh(): Promise<void> {
        return this._loadDataSourceCore();
    }

    clearData() {
        this.gridData = undefined;
    }

    private _loadDataSourceCore() {
        this.loadingData = true;

        let params: { [param: string]: any };
        if (this.infoModel && this.queryParams) {
            params = {};
            this.queryParams.split(',').map((key) => {
                params[key] = this.infoModel[key];
            });
        }

        return this.uow.loadDataSource(this.searchModel, this.parentId, params)
            .then(result => {
                if (this.group.length > 0) {
                    const state: State = {
                        skip: this.skip,
                        take: this.take,
                        sort: this.sort,
                        group: this.group
                    };
                    this.gridData = process(result.data, state);
                } else {
                    this.gridData = {
                            data: result.data,
                            total: result.recordsTotal,
                        };
                }

                this.filterExist = result.filterExist;

                return result;
            }).catch((err: Error) => {
                throw err;
            }).finally(() => {
                this.loadingData = false;
            });
    }

    // QuickSearch
    public onQuickSearchClick(ev: MouseEvent): void {
        ev.stopImmediatePropagation();
    }

    public onQuickSearchKeydown(ev: KeyboardEvent, wrapper: HTMLDivElement): void {
        if (ev.key === "Escape") {
            wrapper.focus();
        }
        if (ev.key === "ArrowLeft" || ev.key === "ArrowRight") {
            ev.stopImmediatePropagation();
        }
    }

    public onQuickSearchValueChange(value: any) {
        this.searchModel['quickSearch'] = value;

        this._loadDataSourceCore();
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
            const dataItem = this.clickedRowItem["dataItem"];
            const column = this.clickedRowItem["column"];
            const pathUrl = column["link"] ? column["link"] : this.pathUrl;

            this.editEntityBy(pathUrl, dataItem[this.searchModel.dataKey]);
        }
    }

    editEntity(url: string, id: number) {
        if (this.parentId)
            this.router.navigate([url, id, this.parentId]);
        else
            this.router.navigate([url, id]);
    }

    filterPanelChange(value: boolean): void {
        this.filterPanelVisible = !this.filterPanelVisible;
    }

    getBackgroundColor(dataItem: any, enumField: string, color: number) {
        const fieldValue: number = dataItem[enumField];
        const value = 90 - (fieldValue * 10);

        if (value < 50)
            return { 'background-color': `hsl(${color},100%,${value}%)`, 'color': 'white' };
        else
            return { 'background-color': `hsl(${color},100%,${value}%)` };
    }

    // if goBackButtonVisible true, parentUrl must exist
    goBack() {
        if (this.parentUrl)
            if (this.parentId)
                this.router.navigate([this.parentUrl, this.parentId]);
            else
                this.router.navigate([this.parentUrl]);
    }

    newRecord() {
        this.createEntityBy();
    }

    createEntity() {
        if (this.infoModel && this.queryParams) {
            const params: any = {};
            this.queryParams.split(',').map((key) => {
                params[key] = this.infoModel[key];
            });

            if (this.parentId)
                this.router.navigate([this.pathUrl, 0, this.parentId], { queryParams: params });
            else
                this.router.navigate([this.pathUrl, 0], { queryParams: params });
        } else {
            if (this.parentId)
                this.router.navigate([this.pathUrl, 0, this.parentId]);
            else
                this.router.navigate([this.pathUrl, 0]);
        }

    }

    exportToPdf() {
        this.onExportToPdf.emit();
    }

    columnButtonClick(event: Event, action: string, dataItem: any) {
        event.stopPropagation();
        this.onColumnButtonClick.emit({ action, dataItem });
    }

    get canDelete() {
        return this.selectedKeys.length > 0;
    }

    deleteRecord() {
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

        const afterDeletion = () => {
            this.ClearSelectedKeys();
            this._loadDataSourceCore().then(() => {
                this.toastrService.success(this.config.getTranslation('message.deletionCompleted'));
            });
        };

        lastValueFrom(dialog.result)
            .then((result: any) => {
                if (result['dialogResult'] === DialogResult.Ok) {
                    if (this.selectedKeys.length > 1 && this.multipleDelete) {
                        this.uow.deleteSelected(this.selectedKeys)
                            .then(() => {
                                afterDeletion();
                            }).catch((err: Error) => {
                                throw err;
                            });
                    } else {
                        this.uow.delete(this.selectedKeys[0])
                            .then(() => {
                                afterDeletion();
                            }).catch((err: Error) => {
                                throw err;
                            });
                    }
                }
            });
    }

    //PersistState
    onReorder(e: any): void {
        const reorderedColumn = this.searchModel.columns.splice(e.oldIndex - 1, 1);
        this.searchModel.columns.splice(e.newIndex - 1, 0, ...reorderedColumn);
    }

    onResize(e: any): void {
        e.forEach((item: any) => {
            this.searchModel.columns.find((col) => col.field === item.column.field).width = item.newWidth;
        });
    }

    onVisibilityChange(e: any): void {
        e.columns.forEach((column: any) => {
            this.searchModel.columns.find((col) => col.field === column.field).hidden = column.hidden;
        });
    }

    saveState() {
        this.searchModel.start = 0;
        this.searchModel.quickSearch = null;
        this.persistStateUow.saveState(this.searchModel, this.searchModel.__entityType)
            .then(() => {
                this.toastrService.success(this.config.getTranslation('message.successfullyCompleted'));
            }).catch((err: Error) => {
                throw err;
            });;
    }

    removeState() {
        this.persistStateUow.removeState(this.searchModel.__entityType)
            .then((removed) => {

                if (!removed) return;

                this.router.navigateByUrl('/office', { skipLocationChange: true })
                    .then(() => this.router.navigate([this.pathUrl]))
                    .then(() => {
                        this.ClearSelectedKeys();
                        this.toastrService.success(this.config.getTranslation('message.deletionCompleted'));
                    });
            }).catch((err: Error) => {
                throw err;
            });
    }

    filterSaveState(inputs: IFormlyFormInputs) {
        this.persistStateUow.saveState(inputs.model, inputs.model.__entityType)
            .then(() => {
                inputs.form.markAsPristine();
                this.refresh().then(() => {
                    this.ClearSelectedKeys();
                    this.toastrService.success(this.config.getTranslation('message.successfullyCompleted'));
                });

            }).catch((err: Error) => {
                throw err;
            });
    }

    filterRemoveState(inputs: IFormlyFormInputs) {
        this.persistStateUow.removeState(inputs.model.__entityType)
            .then((removed) => {
                const clone = Object.assign({}, inputs.default);

                inputs.options.resetModel(clone);
                inputs.form.markAsPristine();

                if (!removed) return;

                this.refresh().then(() => {
                    this.toastrService.success(this.config.getTranslation('message.deletionCompleted'));
                });
            }).catch((err: Error) => {
                throw err;
            });
    }

    //menu
    selectMenuItem({ item }) {
        this.onSelectMenuItem.emit(item);
    }
}
