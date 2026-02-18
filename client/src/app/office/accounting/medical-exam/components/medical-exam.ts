import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";

import { DataResult, SortDescriptor, orderBy, process } from '@progress/kendo-data-query';
import { saveAs } from '@progress/kendo-file-saver';
import * as XLSX from 'xlsx';
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";
import { MedicalExamUnitOfWork } from '@officeNg';
import { FileRestrictions } from "@progress/kendo-angular-upload";
//import { getFieldProperties } from "@formlyNg";

@Component({
    selector: "medical-exam",
    templateUrl: "./medical-exam.html",
    animations: [stateAnimation]
})
export class MedicalExamComponent implements OnInit, OnDestroy {
    @ViewChild('fileSelect', { read: ElementRef }) fileSelect!: ElementRef;
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
    fileRestrictions: FileRestrictions = {
        allowedExtensions: [".xlsm", ".xlsx"],
    };

    public sort: SortDescriptor[] = [
        { field: "exam", dir: "asc" }
    ];

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

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

    public sortChange(sort: SortDescriptor[]): void {
        this.sort = sort;
        this.gridData = orderBy(this.gridData, this.sort);
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

    calc(event: Event) {
        event.stopImmediatePropagation();
        const uploadInput = document.querySelector('input[type="file"]') as HTMLInputElement;
        if (uploadInput) {
            uploadInput.click();
        }
    }

    // Handle file changes when files are selected.
    onFileChange(files: File[]) {
        if (files && files.length > 0) {
            const reader = new FileReader();

            this.gridData = undefined;
            this.loading = true;
            reader.onload = (e: any) => {
                this.getData(e.target.result)
                    .then((results: any[]) => {
                        this.gridData = results;
                    }).finally(() => {
                        this.loading = false;
                    });
            };

            reader.readAsBinaryString(files[0]);
        }
    }

    getData(binaryData: string) {
        return new Promise((resolve, reject) => {
            try {
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
                        category: price <= 50 ? "Βιοχημική" : "Μοριακή",
                        symmetochi: group.aggregates.symmetochi.sum,
                        foreas: group.aggregates.foreas.sum,
                        price: price,
                        count: group.aggregates.printname.count
                    };
                });
                resolve(grouped);
            }
            catch (error) {
                reject('Error parsing file');
                console.error('Error parsing file', error);
            }
        });
    }

}
