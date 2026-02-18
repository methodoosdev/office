import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { FormGroup } from "@angular/forms";

import { CellClickEvent } from "@progress/kendo-angular-grid";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { saveAs } from '@progress/kendo-file-saver';
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { DialogResult, IFormlyFormInputs, MyDataUnitOfWork } from '@officeNg';
import { TranslationService } from "@core";
import { getFieldProperties } from "@formlyNg";
import { lastValueFrom } from "rxjs";
import { MyDataDialogComponent } from "./myData-dialog";
import { MyDataDetailDialogComponent } from "./myData-detail-dialog";
import { CompositeFilterDescriptor, filterBy } from "@progress/kendo-data-query";

@Component({
    selector: "myData",
    templateUrl: "./myData.html",
    encapsulation: ViewEncapsulation.None,
    //styles: [`
    //    .k-myData .k-grid .k-grid-header .k-table-th {
    //        font-weight: 500;
    //    }
    //    .k-myData .k-master-row.k-table-row .master-h-column.k-table-th {
    //        background-color: aliceblue;
    //    }
    //`],
    //host: {
    //    '[class.k-myData]': 'true'
    //},
    animations: [stateAnimation]
})
export class MyDataComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];
    detailColumns: any[];

    title: string;
    calcLabel: string;
    saveLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;
    collapseLabel: string;
    expandLabel: string;

    actions: any[];
    infoInputs: IFormlyFormInputs;
    dialogFormModel: any;
    detailDialogFormModel: any;
    myDataModel: any;
    myDataDetailModel: any;
    expandedDetailKeys: number[] = [];
    allDetailKeys: number[] = [];
    filter: CompositeFilterDescriptor;

    constructor(
        private uow: MyDataUnitOfWork,
        private dialogService: DialogService,
        private toastrService: ToastrService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.cancel');
        this.calcLabel = this.translationService.translate('common.calc');
        this.saveLabel = this.translationService.translate('common.save');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
        this.collapseLabel = this.translationService.translate('common.collapse');
        this.expandLabel = this.translationService.translate('common.expand');

        this.actions = [
            { text: this.yesLabel, themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: this.noLabel, dialogResult: DialogResult.Cancel }
        ];
    }
    
    modelChangeEvent(value: any) {
        this.gridData = undefined;
        this.expandedDetailKeys = [];
    }

    expandCollapse() {
        new Promise((resolve, reject) => {
            setTimeout(() => {
                this.expandedDetailKeys = this.expandedDetailKeys.length > 0 ? [] : Object.assign(this.allDetailKeys);
                resolve({});
            }, 0);
        });
    }

    filterChange(filter: CompositeFilterDescriptor): void {
        this.filter = filter;
    }

    ngOnInit(): void {

        this.uow.loadProperties()
            .then((result: any) => {
                result.infoModel['startDate'] = new Date(result.infoModel['startDate']);
                result.infoModel['endDate'] = new Date(result.infoModel['endDate']);

                this.title = result.tableModel.customProperties.title;
                this.columns = result.tableModel.customProperties.columns;
                this.detailColumns = result.detailTableModel.customProperties.columns;

                const { fields, ...customPropertiesRest } = result.infoFormModel.customProperties;
                const properties = getFieldProperties(fields);

                const infoInputs = {
                    form: new FormGroup({}),
                    options: {},
                    fields: fields,
                    model: result.infoModel,
                    properties: properties,
                    customProperties: customPropertiesRest
                };
                this.infoInputs = infoInputs;

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        const exist = typeof this.infoInputs?.model.traderId === "number";
        return exist ? this.infoInputs?.model.traderId > 0 : false;
    }

    expandDetailsBy = (dataItem: any): number => {
        return dataItem.id;
    };

    calc() {
        this.gridData = undefined;

        this.uow.loadDataSource(this.infoInputs.model)
            .then((result: any) => {
                const list: any[] = result.list;

                list.forEach((x) => {
                    x.issueDate = new Date(x.issueDate);
                });
                this.allDetailKeys = list.map(x => x.id);

                this.gridData = result.list;
                this.dialogFormModel = result.dialogFormModel;
                this.detailDialogFormModel = result.detailDialogFormModel;

                if (result.message)
                    this.toastrService.info(result.message);

            }).catch((error: Error) => {
                throw error;
            });
    }

    save() {
        const data: any[] = [];
        const gridData = filterBy(this.gridData, this.filter);

        gridData.forEach((model) => {

            model.details.forEach((detail) => {
                const item: any = {};

                item["lastDateOnUtc"] = new Date();
                item["counterpartVat"] = model.vatNumber;
                item["invoiceType"] = model.invoiceType;
                item["series"] = model.series;
                item["branch"] = model.branch;
                item["paymentMethodId"] = model.paymentMethodId;
                item["vatProvisionId"] = model.vatProvisionId;
                item["seriesId"] = model.seriesId;
                item["docTypeId"] = model.docTypeId;
                item["isIssuer"] = model.isIssuer;

                item["vatCategoryId"] = detail.vatCategoryId;
                item["taxCategoryId"] = detail.taxCategoryId;
                item["productCode"] = detail.productCode;

                data.push(item);
            });
        });

        this.uow.save(data, this.infoInputs.model.traderId)
            .then((result: any) => {
                this.toastrService.success(this.translationService.translate('message.successfullySaved'));
            }).catch((error: Error) => {
                throw error;
            });
    }

    private getOptionLabel(inputs: IFormlyFormInputs, property: string) {
        const options = inputs.properties[property].props.options as any[];
        const item = options.find((x) => {
            return x.value == inputs.model[property];
        });

        return item ? item.label : null;
    }


    myDataDialogOpen(model: any, docTypeId: number) {

        const dialogRef: DialogRef = this.dialogService.open({
            content: MyDataDialogComponent,
            actions: this.actions,
            width: 550, height: 360, minWidth: 360, actionsLayout: 'end'
        });

        const { fields, ...customProperties } = this.dialogFormModel.customProperties;
        const properties = getFieldProperties(fields);

        const inputs = {
            form: new FormGroup({}),
            options: {},
            fields: fields,
            model: model,
            properties: properties,
            customProperties: customProperties
        };

        properties['seriesId'].props['options'] = docTypeId == 8 ? customProperties.expenses : customProperties.series;
        
        const dialog = dialogRef.content.instance as MyDataDialogComponent;
        dialog.title = customProperties.title
        dialog.inputs = inputs;

        lastValueFrom(dialogRef.result).then((result: any) => {
            if (result.dialogResult === DialogResult.Ok) {

                const item = this.myDataModel;

                item.paymentMethodId = inputs.model.paymentMethodId;
                item.paymentMethodName = this.getOptionLabel(inputs, 'paymentMethodId');
                item.seriesId = inputs.model.seriesId;
                item.seriesName = this.getOptionLabel(inputs, 'seriesId');
                item.vatProvisionId = inputs.model.vatProvisionId;
                item.vatProvisionName = this.getOptionLabel(inputs, 'vatProvisionId');
                item.exportToExcel = inputs.model.exportToExcel;
            }
        });
    }

    myDataDetailDialogOpen(model: any, docTypeId: number) {

        const dialogRef: DialogRef = this.dialogService.open({
            content: MyDataDetailDialogComponent,
            actions: this.actions,
            width: 550, height: 360, minWidth: 360, actionsLayout: 'end'
        });

        const { fields, ...customProperties } = this.detailDialogFormModel.customProperties;
        const properties = getFieldProperties(fields);

        const inputs = {
            form: new FormGroup({}),
            options: {},
            fields: fields,
            model: model,
            properties: properties,
            customProperties: customProperties
        };

        properties['productCode'].props['options'] = docTypeId == 8 ? customProperties.specialSuppliers : customProperties.products;

        const dialog = dialogRef.content.instance as MyDataDetailDialogComponent;
        dialog.title = customProperties.title
        dialog.inputs = inputs;

        lastValueFrom(dialogRef.result).then((result: any) => {
            if (result.dialogResult === DialogResult.Ok) {

                const item = this.myDataDetailModel;
                
                item.productCode = inputs.model.productCode;
                item.productCodeName = this.getOptionLabel(inputs, 'productCode');
            }
        });
    }

    exportToExcel() {
        const obj: any[] = [];

        this.gridData.forEach((model) => {

            model.details.forEach((detail) => {
                const item = {
                    seriesId: model.seriesId,
                    createdOnUtc: model.issueDate,
                    invoice: model.series + ' ' + model.aa,
                    traderCode: model.traderCode,
                    productCode: detail.productCode,
                    quantity: 1,
                    netValue: model.totalNetValue,
                    vatId: 0,
                    currencyId: 0,
                    vatProvisionId: model.vatProvisionId,
                    paymentMethodId: model.paymentMethodId
                };
                obj.push(item);
            });
        });

        this.uow.exportToExcel(obj)
            .then((result: any) => {
                saveAs(result, "myData.xlsx");

            }).catch((error: Error) => {
                throw error;
            });
    }

    //row click
    onCellClick(event: CellClickEvent) {
        if (event.type == 'click')
            this.myDataModel = event.dataItem;
    }

    onDblClick(event: Event) {
        event.stopPropagation(); // do not remove it
        if (this.myDataModel) {

            const model = {
                docTypeId: this.myDataModel.docTypeId,
                paymentMethodId: this.myDataModel.paymentMethodId,
                seriesId: this.myDataModel.seriesId,
                vatProvisionId: this.myDataModel.vatProvisionId,
                exportToExcel: this.myDataModel.exportToExcel
            };

            this.myDataDialogOpen(model, this.myDataModel.docTypeId);
        }
    }

    onDetailCellClick(event: CellClickEvent) {
        if (event.type == 'click')
            this.myDataDetailModel = event.dataItem;
    }

    onDetailDblClick(event: Event) {
        event.stopPropagation(); // do not remove it
        if (this.myDataDetailModel) {

            const model = {
                productCode: this.myDataDetailModel.productCode
            };

            const parent = this.gridData.find((x) => x.id == this.myDataDetailModel.parentId);
            this.myDataDetailDialogOpen(model, parent.docTypeId);
        }
    }

}
