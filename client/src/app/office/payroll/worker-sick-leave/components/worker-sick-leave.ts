import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { WorkerSickLeaveUnitOfWork } from "@officeNg";
import { ApiConfigService, TranslationService } from "@core";


@Component({
    selector: "worker-sick-leave",
    templateUrl: "./worker-sick-leave.html",
    animations: [stateAnimation]
})

export class WorkerSickLeaveComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    searchLabel: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    constructor(
        private uow: WorkerSickLeaveUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService,
        private api: ApiConfigService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.searchLabel = this.translationService.translate('common.search');
        this.calcLabel = this.translationService.translate('common.calc');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
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

    onQuickSearchValueChange(value: any) {
        this.searchModel['quickSearch'] = value;

        this.calc();
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    ngOnInit(): void {

        //this.traderName = this.api.configuration.trader?.fullName;

        this.uow.loadProperties()
            .then((data: any) => {
                data.searchModel['to'] = new Date(data.searchModel['to']);

                this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

                this.searchFields = data.searchModel.customProperties.fields;
                this.searchModel = data.searchModel;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });

    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        return this.searchModel && this.searchModel.traderId > 0;
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((data: any) => {
                this.gridData = data;
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }
}
