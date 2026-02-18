import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { WorkerLeaveUnitOfWork } from "@officeNg";
import { ApiConfigService, TranslationService } from "@core";


@Component({
    selector: "worker-leave",
    templateUrl: "./worker-leave.html",
    animations: [stateAnimation],
})

export class WorkerLeaveComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    constructor(
        private uow: WorkerLeaveUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService,
        private api: ApiConfigService) {

        this.title = this.translationService.translate('menu.workerLeave');
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
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

    colIndex: number
    onExcelExport(data: any) {

        const rows = data.workbook.sheets[0].rows;
        rows.forEach((row: any) => {
            // Store the price column index
            if (row.type === 'header') {
                row.cells.forEach((cell: any, index: any) => {
                    if (cell.value === this.translationService.translate('common.hireDate')) {
                        this.colIndex = index;
                        return;
                    }
                });
            }
            // Use the column index to format the price cell values
            if (row.type === 'data') {
                row.cells.forEach((cell: any, index: any) => {
                    if (index === this.colIndex) {
                        const dateValue = cell.value.substring(0, 10); // Extract the date part from the string
                        const parts = dateValue.split('-'); // Split the date into year, month, and day parts
                        const formattedDate = `${parts[2]}/${parts[1]}/${parts[0]}`; // Format the date as 'dd/MM/yyyy'
                        cell.value = formattedDate; // Update the cell value
                    }
                });
            }
        });  

    }
}
