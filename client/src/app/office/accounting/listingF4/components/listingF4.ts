import { Component, Inject, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { HttpErrorResponse } from "@angular/common/http";

import { AddEvent, RemoveEvent } from "@progress/kendo-angular-grid";
import { groupBy, AggregateDescriptor, GroupResult, GroupDescriptor } from "@progress/kendo-data-query";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { ChatHubService, TranslationService } from "@core";
import { ListingF4UnitOfWork } from "@officeNg";

@Component({
    selector: "listingF4",
    templateUrl: "./listingF4.html",
    encapsulation: ViewEncapsulation.None,
    styles: [`
        .k-command-cell { 
            text-align: center !important; 
        }
        tr.k-grouping-row { 
            display: none;
        }
    `],
    providers: [ChatHubService],
    animations: [stateAnimation]
})
export class ListingF4Component implements OnInit, OnDestroy {
    excelFileName: string = 'listingF4.xlsx';
    animate: boolean = true;
    columns: any[];
    dialogLabels: any[];

    insertLabel: string;
    title: string;
    calcLabel: string;
    submitedLabel: string;
    importLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;
    errorLabel: string;
    yesLabel: string;
    noLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    isNew = false;
    listModel: any;

    dataSource: GroupResult[];
    _gridData: any[];

    get gridData(): any[] {
        return this._gridData;
    }

    set gridData(value: any[]) {
        this._gridData = value;
        this.dataSource = value ? groupBy(value, this.group) : null;
    }

    _canSubmitTo = true;
    _canRetrieve = true;

    aggregates: AggregateDescriptor[] = [
        { field: "goods", aggregate: "sum" },
        { field: "triangleExchange", aggregate: "sum" },
        { field: "services", aggregate: "sum" },
        { field: "products4200", aggregate: "sum" }
    ];
    group: GroupDescriptor[] = [{ field: 'group', aggregates: this.aggregates }];

    constructor(
        @Inject('BASE_URL') private baseUrl: string,
        private uow: ListingF4UnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService,
        private chatHubService: ChatHubService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.insertLabel = this.translationService.translate('common.insert');
        this.title = this.translationService.translate('title.listingF4');
        this.calcLabel = this.translationService.translate('common.calc');
        this.submitedLabel = this.translationService.translate('common.submited');
        this.importLabel = this.translationService.translate('common.retrieve');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    ngOnDestroy(): void {
        //this.chatHubService.stop();
    }

    ngOnInit(): void {
        //this.chatHubService.start();

        this.uow.loadProperties()
            .then((result: any) => {
                result.searchModel['period'] = new Date(result.searchModel['period']);

                this.columns = result.tableModel.customProperties.columns;
                this.errorLabel = this.columns[0].title
                this.dialogLabels = result.dialogLabels;

                this.searchFields = result.searchModel.customProperties.fields;
                this.searchModel = result.searchModel;


            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    calc() {
        this.uow.loadData(this.searchModel, this.chatHubService?.connectionId)
            .then((result: any) => {
                this.gridData = result.data;
                this.columns[0].title = this.errorLabel;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    submitTo() {
        const period = this.searchModel['period'] as Date;
        const data = {
            traderId: this.searchModel.traderId,
            year: period.getFullYear(),
            month: period.getMonth() + 1,
            data: this.gridData
        };

        this._canSubmitTo = false;
        this.uow.submitTo(data, this.chatHubService?.connectionId)
            .then(() => {
                const successfullySubmited = this.translationService.translate('message.successfullySubmited');
                this.toastrService.success(successfullySubmited);
            })
            .catch((e: HttpErrorResponse) => {
                this.toastrService.error(e.error);
            })
            .finally(() => {
                this._canSubmitTo = true;
                window.open(`${this.baseUrl}docs/customer-activity-log`, "_blank");
            });
    }

    retrieve() {
        const date = this.searchModel.period as Date;

        this._canRetrieve = false;
        this.uow.retrieve(this.searchModel.traderId, date.getFullYear(), date.getMonth() + 1, this.chatHubService?.connectionId)
            .then((result: any) => {
                this.gridData = result.data;
                this.columns[0].title = this.translationService.translate('common.kind');
            }).catch((err: any) => {
                this.toastrService.error(err.error);
            })
            .finally(() => {
                this._canRetrieve = true;
            });
    }

    get canSubmitTo() {
        return this._canSubmitTo && this.gridData && this.gridData.length > 0;
    }

    get canRetrieve() {
        return this._canRetrieve;
    }

    get canCalc() {
        return this.searchModel && this.searchModel.traderId > 0;
    }

    //dialog
    public addHandler(): void {
        this.listModel = {
            error: '',
            group: this.translationService.translate('common.home'),
            countryCode: '',
            vat: '',
            vatNumber: '',
            goods: 0,
            triangleExchange: 0,
            services: 0,
            products4200: 0,
        };
        this.isNew = true;
    }

    public editHandler(args: AddEvent): void {
        this.listModel = args.dataItem;
        this.isNew = false;
    }

    public cancelHandler(): void {
        this.listModel = undefined;
    }

    public saveHandler(model: any): void {
        const dataItem = this.gridData.find(x => x.vat === model.vat && x.group === model.group);

        if (this.isNew) {
            if (dataItem) {
                this.toastrService.error(this.translationService.translate('error.uniqueVatValidation'));
                return;
            } else {
                this.gridData.push(model);
            }
        } else {
            Object.assign(dataItem, model);
        }

        this.gridData = this.gridData;
        this.listModel = undefined;
    }

    public removeHandler(args: RemoveEvent): void {
        const filter = args.dataItem.vat + args.dataItem.group;
        const index = this.gridData.map(e => e.vat + e.group).indexOf(filter, 0);
        if (index > -1) {
            this.gridData.splice(index, 1);
            this.gridData = this.gridData;
        }
    }
}
