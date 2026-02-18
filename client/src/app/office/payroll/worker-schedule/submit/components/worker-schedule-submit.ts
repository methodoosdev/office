import { Component, NgZone, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { IntlService } from '@progress/kendo-angular-intl';
import { GridDataResult, CancelEvent, EditEvent, GridComponent, SaveEvent, SelectableSettings, RowClassArgs, GroupRowArgs } from '@progress/kendo-angular-grid';
import { process, AggregateDescriptor, GroupDescriptor, CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { Observable, map, take } from 'rxjs';

import { CanComponentDeactivate } from '@jwtNg';
import { slideInOutAnimation, stateAnimation} from '@primeNg';
import { DialogResult, DynamicDialogService, WorkerScheduleSubmitUnitOfWork } from '@officeNg';
import { TranslationService, UtilsService } from '@core';
import { EditService, WorkerScheduleDate } from './edit-service';

@Component({
    selector: 'worker-schedule-submit',
    templateUrl: "./worker-schedule-submit.html",
    providers: [EditService],
    animations: [stateAnimation, slideInOutAnimation],
    encapsulation: ViewEncapsulation.None,
    styles: [`  
        .worker-schedule-wrapper .gold .workingDateColumn {
            color: #4caf50;
            font-weight: 500;
        }
        .worker-schedule-wrapper .green .workingDateColumn {
            color: #ff9800;
            font-weight: 500;
        }
        .worker-schedule-wrapper kendo-grid-group-panel div[kendodraggable] a.k-button.k-icon-button {
        display: none;
        }
        /*kendo-grid-group-panel div[kendodraggable]{
        pointer-events: none;
        }*/
        .worker-schedule-wrapper kendo-timepicker.ng-dirty input.k-input-inner {
            font-weight: 600;
        }
        .worker-schedule-wrapper .k-grid .k-master-row td.time-column {
            padding: 0;
        }
        .worker-schedule-wrapper .k-grid th, .worker-schedule-wrapper .k-grid td {
            padding: 3px 12px;
        }
        .worker-schedule-wrapper .wrap {
            display: flex;
            flex-wrap: wrap;
        }
        .worker-schedule-wrapper .wrap-div {
            display: flex;
        }
        .worker-schedule-wrapper .wrap-control {
            margin-right: 12px;
        }
        .worker-schedule-wrapper .wrap-control .k-label {
            font-size: 14px;
        }
        .worker-schedule-wrapper .wrap.cell-template .wrap-div {
            width: 102px;
        }
        .worker-schedule-wrapper .wrap.cell-template .wrap-div .wrap-control {
            width: 100%;
        }
        .worker-schedule-wrapper .wrap.cell-template .wrap-div .wrap-control .time {
            border: 1px solid rgba(0, 0, 0, 0.08);
            padding: 2px 8px;
            border-radius: 4px;
        }
        .worker-schedule-wrapper .k-grid .k-command-cell {
            text-align: center;
        }
        /*.k-grid .k-command-cell > .k-button + .k-button {
            margin-left: 0;
            margin-inline-start: 0;
        }*/
        .danger .k-dialog-titlebar {
            background-color: #f44336;
        }
        .worker-schedule-wrapper .wrap-div.checkbox .fa.fa-fw.fa-check {
            color: limegreen;
        }

        .worker-schedule-wrapper .sticky-wrapper {
            position: -webkit-sticky;
            position: sticky;
            top: 60px;

            z-index: 99;
            background-color: #f5f5f5;
        }
  `]
})
export class WorkerScheduleSubmitComponent implements OnInit, CanComponentDeactivate {
    @ViewChild('myForm', { static: false }) myForm!: NgForm;
    @ViewChild('dialogTemplate') dialogTemplate!: TemplateRef<any>;
    @ViewChild(GridComponent) grid: GridComponent;
    view: Observable<GridDataResult>;
    expandedKeys: { field: string; value: string }[] = [];
    initiallyExpanded = false;

    editedRowIndex: number;
    editedDataItem: WorkerScheduleDate;

    animate: boolean = true;
    shiftPanelVisible: boolean = true;
    breakControlsVisible: boolean = false;
    emptyDate = "2000-02-01T22:00:00Z";
    minDate: Date = new Date(2000, 2, 2, 0, 0, 0);
    maxDate: Date = new Date(2002, 2, 2, 23, 59, 59);
    aggregates: AggregateDescriptor[] = [
        { field: "dailyTotalHours", aggregate: "sum" }
    ];
    byWorkingDate: GroupDescriptor[] = [{ field: 'workingDate', aggregates: this.aggregates }];
    byWorkerCardName: GroupDescriptor[] = [{ field: 'workerCardName', aggregates: this.aggregates }];
    dataKey = 'id';
    selectedKeys: any[] = [];
    selectableSettings: SelectableSettings = {
        enabled: true,
        checkboxOnly: true,
        mode: `multiple`,
        cell: false,
        drag: false
    };
    filter: CompositeFilterDescriptor = {
        logic: 'and',
        filters: []
    };
    quickSearch: string = '';
    canEdit: boolean;

    groups: GroupDescriptor[];

    parentUrl: string;
    pathUrl = 'office/worker-schedule-submit';

    searchLabel: string;
    saveLabel: string;
    cancelLabel: string;
    checkLabel: string;
    noRecordsLabel: string;
    informLabel: string;
    nonstopGroupLabel: string;
    splitGroupLabel: string;
    overtimeGroupLabel: string;
    collapseLabel: string;
    expandLabel: string;
    autofitColumnsLabel: string;

    title: string;
    traderName: string;
    breakLimit: number;

    parentId: number;
    shifts: any[];

    columns: any;
    dialogContent: string;

    constructor(
        private ngZone: NgZone,
        private route: ActivatedRoute,
        private router: Router,
        private uow: WorkerScheduleSubmitUnitOfWork,
        private editService: EditService,
        private dynamicDialogService: DynamicDialogService,
        private toastrService: ToastrService,
        private utils: UtilsService,
        private intl: IntlService,
        private translationService: TranslationService) {

        this.searchLabel = this.translationService.translate('common.search');
        this.saveLabel = this.translationService.translate('common.save');
        this.cancelLabel = this.translationService.translate('common.cancel');
        this.checkLabel = this.translationService.translate('common.check');
        this.noRecordsLabel = this.translationService.translate('common.noRecords');
        this.informLabel = this.translationService.translate('common.inform');
        this.nonstopGroupLabel = this.translationService.translate('common.nonstop');
        this.splitGroupLabel = this.translationService.translate('common.split');
        this.overtimeGroupLabel = this.translationService.translate('common.overtime');
        this.collapseLabel = this.translationService.translate('common.collapse');
        this.expandLabel = this.translationService.translate('common.expand');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    public ngOnInit(): void {


        this.route.params.forEach((params: any) => {

            this.parentId = +params.id;

            this.uow.getEntity(this.parentId, null, {})
                .then(result => {
                    const columns = result.model.columns;
                    const data: any[] = result.data;

                    this.editService.purgeData(data, columns);

                    this.title = result.model.title;
                    this.traderName = result.model.traderName;
                    this.breakLimit = result.model.breakLimit;
                    this.canEdit = result.model.canEdit;
                    this.shifts = result.model.shifts;

                    this.parentUrl = result.model.isTrader ? 'office/worker-schedule-by-trader' : 'office/worker-schedule-by-employee';

                    this.columns = columns;

                    this.editService.init(data);

                    this.groupValueChange(false);
                })
                .catch((err: Error) => {
                    //this.navigationBack();
                    throw err;
                });
        });
    }

    get hasChanges(): boolean {
        return this.myForm?.form?.dirty || false;
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        if (!this.hasChanges) { return true; }

        const warning = this.translationService.translate('common.warning');
        const canDeactivate = this.translationService.translate('message.canDeactivate');

        return this.dynamicDialogService.open(warning, canDeactivate)
            .then((result) => {
                if (result === DialogResult.Ok) {
                    const item = this.editService.data.find(item => item.id === this.editedDataItem.id);
                    return this.save(item).then((value) => value);
                }
                if (result === DialogResult.No) {
                    return true;
                }

                return false;
            });
    }

    breakControlsChange(value: boolean) {
        this.breakControlsVisible = !this.breakControlsVisible;
    }

    public onGroupChange(groups: GroupDescriptor[]): void {
        // set aggregates to the returned GroupDescriptor
        this.groups.map((group) => (group.aggregates = this.aggregates));

        this.groups = groups;
        this.loadDataSource();
    }

    groupValueChange(value: boolean) {
        if (value) {
            this.groups = this.byWorkingDate;
            this.columns.workerCardName.hidden = false;
            this.columns.workingDate.hidden = true;
        } else {
            this.groups = this.byWorkerCardName;
            this.columns.workerCardName.hidden = true;
            this.columns.workingDate.hidden = false;
        }
        this.loadDataSource();
    }

    //expand collapse
    isGroupExpanded = (rowArgs: GroupRowArgs): boolean => {
        const matchKey = this.expandedKeys.some((groupKey) =>
            groupKey.field === rowArgs.group.field && groupKey.value === rowArgs.group.value
        );

        return ((this.initiallyExpanded && !matchKey) || (!this.initiallyExpanded && matchKey));
    };

    expandedValueChange(value: boolean) {
        this.initiallyExpanded = !this.initiallyExpanded;
        this.expandedKeys = [];

        if (value) {
            //this.fitColumns();
        }
    }

    fitColumns(): void {
        this.ngZone.onStable
            .asObservable()
            .pipe(take(1))
            .subscribe(() => {
                this.grid.autoFitColumns();
            });
    }

    expandAll(): void {
        this.initiallyExpanded = true;
        this.expandedKeys = [];
    }

    collapseAll(): void {
        this.initiallyExpanded = false;
        this.expandedKeys = [];
    }

    toggleGroup(rowArgs: GroupRowArgs): void {
        const keyIndex = this.expandedKeys.findIndex((groupKey) =>
            groupKey.field === rowArgs.group.field && groupKey.value === rowArgs.group.value
        );

        if (keyIndex === -1) {
            this.expandedKeys.push({
                field: rowArgs.group.field,
                value: rowArgs.group.value
            });
            //this.fitColumns();
        } else {
            this.expandedKeys.splice(keyIndex, 1);
        }
    }

    private loadDataSource() {

        this.view = this.editService.pipe(
            map((data) => {
                // set aggregates to the returned GroupDescriptor
                this.groups.map((group) => (group.aggregates = this.aggregates));

                //this.expandedKeys = [{ field: 'workerCardName', value: this.editService.data[0]['workerCardName'] }];

                return process(data, { group: this.groups, filter: this.filter })
            })
        );

        return Promise.resolve(null).then(() => {
            this.editService.read();
        });
    }

    onQuickSearchValueChange(value: any) {
        if (value) {
            this.filter = {
                logic: 'and',
                filters: [{ field: 'workerCardName', operator: 'contains', value: value }]
            };
        } else {
            this.filter = {
                logic: 'and',
                filters: []
            };
        }

        this.loadDataSource();
    }

    getBreakItemError(item: WorkerScheduleDate): Boolean {

        const totalHours = this.getHours(item.dailyTotalHours);
        const totalBreak = (this.getHours(item.dailyBreak) * 60) + this.getMinutes(item.dailyBreak);

        if (totalHours >= 4 && totalBreak < 15) {
            return true;
        }
        return false;
    }

    save(item: WorkerScheduleDate) {
        return Promise.resolve(null).then(() => {
            const dailyTotalHoursError = item.dailyTotalHours > (8 * 60 * 60000);

            if (dailyTotalHoursError) {
                const workingDate = this.intl.formatDate(item.workingDate, 'd');
                const titleError = this.translationService.translate('error.dailyTotalHoursError');
                this.toastrService.error(`${item.workerCardName} - ${workingDate}: ${this.getString(item.dailyTotalHours)}`, titleError);

                return false;
            }

            const breakItemError = this.getBreakItemError(item);
            if (breakItemError) {
                const workingDate = this.intl.formatDate(item.workingDate, 'd');

                const titleError = this.translationService.translate('error.dailyBreakError');
                this.toastrService.error(`${item.workerCardName} - ${workingDate}: ${this.getString(item.dailyBreak)} < ${this.getString(15 * 60000)}`, titleError);

                return false;
            }

            const breakError = item.dailyBreak > (this.breakLimit * 60000);
            if (breakError) {
                const titleError = this.translationService.translate('error.breakLimitError');
                const format = this.translationService.translate('error.breakLimitFormat');
                const errorMessage = this.utils.stringFormat(format, this.getString(item.dailyBreak), this.getString(this.breakLimit * 60000));
                this.toastrService.error(errorMessage, titleError);

                return false;
            }

            return this.uow.edit(item)
                .then(() => {
                    //this.loadDataSource();
                    return true;
                })
                .catch((err: Error) => {
                    throw err;
                });
        });
    }

    private sendEmail() {
        this.uow.sendEmail(this.parentId)
            .then(() => {
                const successfullySaved = this.translationService.translate('message.sendScheduleCompleted');
                this.toastrService.success(successfullySaved);
            })
            .catch((err: Error) => {
                throw err;
            });
    }

    canSendEmail(result: any): Promise<boolean> {
        const warning = this.translationService.translate('common.warning');
        const format = this.translationService.translate('message.contractChange');
        const title = `${warning} - ${result.error}`;

        this.dialogContent = this.utils.stringFormat(format, result.workers);

        return this.dynamicDialogService.open(title, this.dialogTemplate, false, true)
            .then((result) => {
                if (result === DialogResult.Ok) {
                    return true;
                }
                if (result === DialogResult.No) {
                    return false;
                }

                return false;
            });
    }

    inform() {
        this.uow.check(this.parentId)
            .then((res) => {
                if (res.valid === false) {
                    if (res.contractChange === true) {
                        this.canSendEmail(res)
                            .then((result) => {
                                if (result === true) this.sendEmail();
                            })
                    } else {
                        this.toastrService.error(res.error);
                    }
                } else {
                    this.sendEmail();
                }
            })
            .catch((err: Error) => {
                throw err;
            });
    }

    check() {
        this.router.navigate(['office/worker-schedule-check', this.parentId]);
    }

    onTimeChange(value: any, dataItem: any, field: string) {
        switch (field) {
            case 'leave':
                if (value?.target?.checked === true) {
                    this.editService.resetDataItem(dataItem);
                    dataItem['leave'] = true;
                }
                break;
            case 'sickLeave':
                if (value?.target?.checked === true) {
                    this.editService.resetDataItem(dataItem);
                    dataItem['sickLeave'] = true;
                }
                break;
            case 'isSplit':
                const properties: string[] = ['splitFromDate', 'splitToDate', 'breakSplitFromDate', 'breakSplitToDate'];

                if (!value?.target?.checked === true) {
                    properties.forEach((prop) => {
                        dataItem[prop] = this.minDate;
                    });
                }
                break;
            case 'nonstopFromDate': case 'nonstopToDate': case 'breakNonstopFromDate': case 'breakNonstopToDate':
            case 'breakNonstop2FromDate': case 'breakNonstop2ToDate':
            case 'splitFromDate': case 'splitToDate': case 'breakSplitFromDate': case 'breakSplitToDate':
                // value can be null
                const date = Date.parse(value?.toString());
                if (isNaN(date)) {
                    dataItem[field] = this.minDate;
                }
                break;
            default:
                break;
        }

        this.editService.resetItem(dataItem);
        this.loadDataSource();
    }

    selectedShiftChange(shift: any): void {
        const items = this.editService.onSelectedShiftChange(this.selectedKeys, shift);
        this.uow.update(items)
            .then(() => {
                this.selectedKeys = [];

                this.router.navigateByUrl(this.parentUrl, { skipLocationChange: true })
                    .then(() => {
                        this.router.navigate([this.pathUrl, this.parentId])
                            .then(() => {
                                const successfullySaved = this.translationService.translate('message.successfullySaved');
                                this.toastrService.success(successfullySaved);
                            });
                    });
            })
            .catch((err: Error) => {
                throw err;
            });
    }

    shiftPanelChange(value: boolean): void {
        this.shiftPanelVisible = !this.shiftPanelVisible;
    }

    navigationBack() {
        this.router.navigate([this.parentUrl]);
    }

    rowCallback = (context: RowClassArgs) => {
        if (context.dataItem.isSaturday) {
            return { gold: true };
        } else if (context.dataItem.isSunday) {
            return { green: true };
        } else {
            return {};
        }
    };
    //Edit Handler
    public editHandler(args: EditEvent): void {
        this.closeEditor(args.sender);
        this.editedRowIndex = args.rowIndex;
        this.editedDataItem = Object.assign({}, args.dataItem);

        args.sender.editRow(args.rowIndex);
    }

    public cancelHandler(args: CancelEvent): void {
        this.closeEditor(args.sender, args.rowIndex);
    }

    public saveHandler(args: SaveEvent): void {
        this.save(args.dataItem).then((result) => {
            if (result) {
                this.editService.save(args.dataItem);
                args.sender.closeRow(args.rowIndex);
                this.myForm.form.markAsPristine();

                this.editedRowIndex = undefined;
                this.editedDataItem = undefined;
            }
        });
    }

    private closeEditor(grid: GridComponent, rowIndex = this.editedRowIndex): void {
        grid.closeRow(rowIndex);
        this.editService.resetItem(this.editedDataItem);
        this.myForm.form.markAsPristine();
        this.editedRowIndex = undefined;
        this.editedDataItem = undefined;
    }
    // Time
    getString(value: number): string {
        const hours = this.getHours(value);
        const minutes = this.getMinutes(value);
        return `${this.intl.formatNumber(hours, "#00")}:${this.intl.formatNumber(minutes, "#00")}`;
    }

    private getHours(hourDiff: number) {
        return Math.floor(hourDiff / 3600 / 1000);
    }
    private getMinutes(hourDiff: number) {
        var minDiff = hourDiff / 60 / 1000; //in minutes
        var hours = Math.floor(hourDiff / 3600 / 1000);
        return minDiff - 60 * hours;
    }

}
