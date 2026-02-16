import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { DataStateChangeEvent, GridDataResult, PagerSettings } from '@progress/kendo-angular-grid';
import { GroupDescriptor, SortDescriptor, State, process, aggregateBy } from '@progress/kendo-data-query';

import { UnitOfWork } from "./unit-of-work";
import { stateAnimation } from "@primeNg";
import { ISearchModel } from './search-model';

@Component({
    template: '<div></div>',
    animations: [stateAnimation]
})
export abstract class FormListBaseComponent implements OnInit {
    @Input() animate: boolean = true;
    @Input() uow: UnitOfWork;
    @Input() parentId: number;
    public checkButtonDisabled = false;

    pageable: PagerSettings | boolean;

    _selectedKeys: any[] = [];
    gridData: GridDataResult;
    draw: any;

    _groupHeaderInputs: string[] = [];
    get groupHeaderInputs() {
        return this._groupHeaderInputs;
    }
    set groupHeaderInputs(value: string[]) {
        this._groupHeaderInputs = value;
        this.groupHeaderInputsChange.emit(value);
    }

    searchModel: ISearchModel;

    skip: number;
    take: number;
    sort: SortDescriptor[];
    group: GroupDescriptor[];

    //events
    @Output() onSelectedKeys: EventEmitter<any[]> = new EventEmitter();
    @Output() onLoadProperties: EventEmitter<any> = new EventEmitter();
    @Output() dataSourceChange: EventEmitter<GridDataResult> = new EventEmitter();
    @Output() groupHeaderInputsChange: EventEmitter<string[]> = new EventEmitter();

    get selectedKeys() {
        return this._selectedKeys;
    }
    set selectedKeys(value: any[]) {
        this._selectedKeys = value;
        this.onSelectedKeys.emit(value);
    }

    ngOnInit(): void {
        this.checkButtonDisabled = true;

        this.uow.loadProperties(this.parentId)
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
                this.onLoadProperties.emit(this.searchModel);

                this.loadDataSource();
            }).catch((err: Error) => {
                Promise.reject(err);
            }).finally(() => {
                this.checkButtonDisabled = false;
            });
    }

    loadDataSource() {
        this._loadDataSourceCore();
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

    protected _loadDataSourceCore() {

        this.uow.loadDataSource(this.searchModel, this.parentId)
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
                this.draw = result.draw ? JSON.parse(result.draw) : null;

                this.groupHeaderInputs = [];
                this.dataSourceChange.emit(this.gridData);

            }).catch((err: Error) => {
                throw err;
            });
    }

    dataStateChange(state: DataStateChangeEvent): void {
        this.skip = state.skip;
        this.take = state.take;
        this.sort = state.sort;
        this.searchModel['sortField'] = state.sort[0].field;
        this.searchModel['sortOrder'] = state.sort[0].dir;
        this.searchModel['start'] = state.skip;
        this.searchModel['length'] = state.take;

        this.loadDataSource();
    }

    get canExport() {
        return this.gridData && this.gridData.data && this.gridData.data.length > 0;
    }

    get canSelect() {
        return this.checkButtonDisabled === false && this.selectedKeys.length > 0;
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

}
