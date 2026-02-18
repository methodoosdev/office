import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";

import { DataResult, process } from '@progress/kendo-data-query';
import { saveAs } from '@progress/kendo-file-saver';
import * as XLSX from 'xlsx';
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";
import { MedicalExamUnitOfWork } from '@officeNg';
//import { getFieldProperties } from "@formlyNg";

@Component({
    selector: "load-excel",
    templateUrl: "./load-excel.html",
    animations: [stateAnimation]
})
export class LoadExcelComponent implements OnInit, OnDestroy {
    @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    loading: boolean = false;

    title: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    constructor(
        private uow: MedicalExamUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.calcLabel = this.translationService.translate('common.calc');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }
    
    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    ngOnDestroy(): void {
        //this.chatHubService.stop();
    }

    ngOnInit(): void {
        this.uow.loadProperties()
            .then((data: any) => {

                this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    calc() {
        this.fileInput.nativeElement.click();
    }

    onFileChange(event: any): void {
        const target: DataTransfer = <DataTransfer>event.target;
        if (target.files.length !== 1) {
            console.error('Please select exactly one file.');
            return;
        }
        this.gridData = undefined;
        this.loading = true;
        const file = target.files[0];
        const reader: FileReader = new FileReader();

        reader.onload = (e: any) => {

            try {
                const binaryData: string = e.target.result;
                const workbook: XLSX.WorkBook = XLSX.read(binaryData, { type: 'binary' });
                const firstSheetName: string = workbook.SheetNames[0];
                const worksheet: XLSX.WorkSheet = workbook.Sheets[firstSheetName];
                const data = XLSX.utils.sheet_to_json(worksheet, { header: 0 });

                const result: DataResult = process(data, {
                    group: [{
                        field: 'printname',
                        aggregates: [
                            { field: "symmetochi", aggregate: "sum" },
                            { field: "foreas", aggregate: "sum" },
                            { field: "price", aggregate: "sum" },
                            { field: "printname", aggregate: "count" }
                        ]
                    }],
                    sort: [{ field: 'printname', dir: 'asc' }]
                });

                const grouped = result.data.map((group) => {
                    const price = group.aggregates.price.sum;

                    return {
                        exam: group.value,
                        category: price <= 50 ? "Biomixaniki" : "Moriaki",
                        symmetochi: group.aggregates.symmetochi.sum,
                        foreas: group.aggregates.foreas.sum,
                        price: price,
                        count: group.aggregates.printname.count
                    };
                });
                this.loading = false;
                this.gridData = grouped;
            }
            catch (error) {
                console.error('Error parsing file', error);
            }
            finally {
                // Finish loading whether success or error
                this.loading = false;
            }

        };

        setTimeout(() => {
            reader.readAsBinaryString(file);
        }, 10);

    }
}
