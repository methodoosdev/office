import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { FmyContributionUnitOfWork } from "@officeNg";
import { TranslationService } from "@core";


@Component({
    selector: "fmy-contribution",
    templateUrl: "./fmy-contribution.html",
    animations: [stateAnimation],
})

export class FmyContributionComponent implements OnInit {
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
        private uow: FmyContributionUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {

        this.title = this.translationService.translate('menu.fmyContribution');
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    isHidden(columnName: string): boolean {
        let hiddenColumns: string[] = [];
        const month = (this.searchModel.period as Date).getMonth() + 1;
        
        if ((month != 12 && month != 4))
            hiddenColumns = ['christmasPresentFmy', 'easterPresentFmy'];
        else if (month == 12)
            hiddenColumns = ['easterPresentFmy'];
        else if (month == 4)
            hiddenColumns = ['christmasPresentFmy'];

        return hiddenColumns.indexOf(columnName) > -1;
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    ngOnInit(): void {

        this.uow.loadProperties()
            .then((data: any) => {
                data.searchModel['period'] = new Date(data.searchModel['period']);

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
        return this.searchModel && this.searchModel.employerIds.length > 0;
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((result: any) => {
                this.columns = result.columns;
                this.gridData = result.list;
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
