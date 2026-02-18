import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";

import { saveAs } from '@progress/kendo-file-saver';
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { ESendUnitOfWork } from '@officeNg';
import { ProgressHubService, TranslationService } from "@core";
//import { getFieldProperties } from "@formlyNg";

@Component({
    selector: "eSend",
    templateUrl: "./eSend.html",
    providers: [ProgressHubService],
    animations: [stateAnimation]
})
export class ESendComponent implements OnInit, OnDestroy {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
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
        private uow: ESendUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService,
        private hubService: ProgressHubService) {

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
        //this.searchForm.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe(() => {
        //    setTimeout(() => {
        //        this.gridData = null;
        //    });
        //});

        //this.chatHubService.start();

        this.uow.loadProperties()
            .then((data: any) => {
                data.searchModel['period'] = new Date(data.searchModel['period']);

                this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

                this.searchFields = data.searchModel.customProperties.fields;
                this.searchModel = data.searchModel;

                //const fieldProperties = getFieldProperties(this.searchFields);

                //fieldProperties['traderId'].props['change'] = (field: FormlyFieldConfig, value: any) => {
                //    const test = value;
                //    const test1 = this;
                //    const test2 = "";
                //};

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        const exist = typeof this.searchModel?.traderId === "number";
        return exist ? this.searchModel.traderId > 0 : false;
    }

    calc() {
        this.uow.loadData(this.searchModel, this.hubService?.connectionId)
            .then((result: any) => {
                this.gridData = result;
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    exportToExcel() {
        this.uow.exportToExcel(this.gridData)
            .then((result: any) => {
                saveAs(result, "eSend.xlsx");

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }
}
