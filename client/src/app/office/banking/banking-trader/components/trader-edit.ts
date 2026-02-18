import { Component, QueryList, ViewChild, ViewChildren } from "@angular/core";
import {
    BankingTraderUnitOfWork, AvailableBankUnitOfWork, UserConnectionBankUnitOfWork, AccountListUnitOfWork, CardListItemUnitOfWork,
    FormEditToken, AfterModelChangeEvent, FormListDetailDialogComponent, UnitOfWork, ColumnButtonClickEvent
} from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";
import { Observable } from "rxjs";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { getFieldProperties, fixKendoDateTime } from "@formlyNg";
import { ToastrService } from "ngx-toastr";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { GridDataResult, GroupKey } from "@progress/kendo-angular-grid";
import { DataResult, GroupResult } from "@progress/kendo-data-query";

@Component({
    selector: "banking-trader-edit",
    templateUrl: "./trader-edit.html"
})

export class BankingTraderEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/banking-trader';
    parentId: number;
    categoryBookTypeId: number;

    availableBanksLabel: string;
    userConnectionsBankLabel: string;

    configForm = new FormGroup({});
    configOptions: FormlyFormOptions = {};
    configFields: FormlyFieldConfig[];
    configModel: any;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public uow: BankingTraderUnitOfWork,
        public availableBankUow: AvailableBankUnitOfWork,
        public userConnectionBankUow: UserConnectionBankUnitOfWork,
        public accountListUow: AccountListUnitOfWork,
        public cardListItemUow: CardListItemUnitOfWork,
        public dialogService: DialogService,
        private toastrService: ToastrService,
        private translationService: TranslationService) { }

    ngOnInit() {
        this.availableBanksLabel = this.translationService.translate('menu.availableBanks');
        this.userConnectionsBankLabel = this.translationService.translate('menu.userConnectionsBank');

        const traderId = +this.route.snapshot.paramMap.get('id');

        this.userConnectionBankUow.getConfig(traderId)
            .then((data: any) => {
                const fieldProperties = getFieldProperties(data.configModel.customProperties.fields);
                const model = fixKendoDateTime(data.configModel, fieldProperties);

                this.configFields = data.configModel.customProperties.fields;
                this.configModel = model;
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return true;
    }

    afterModelChange(e: AfterModelChangeEvent) {
        this.parentId = e.model.id;
        this.categoryBookTypeId = e.model.categoryBookTypeId;
    }

    public getProperties(id: number, parentId: number, uow: UnitOfWork) {
        return uow.getEntity(id, parentId, null)
            .catch((err: Error) => {
                Promise.reject(err);
            });
    }

    onAccountButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'account') {

            this.uow.account(event.dataItem.bankBIC, "demo", event.dataItem.resourceId, this.configModel.dateFrom, this.configModel.dateTo).then((res) => {
                console.log(res);
            });

            //const url = this.router.serializeUrl(
            //    this.router.createUrlTree(
            //        ['/office/script-pivot', event.dataItem.id, this.parentId, this.categoryBookTypeId, this.configModel.year, this.configModel.period, this.configModel.showTypeId, this.configModel.inventory === true ? 1 : 0],
            //        { relativeTo: null }
            //    )
            //);
            //window.open(window.location.origin + url, '_blank');
        }
    }

    onCardButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'card') {
        }
    }

    onConnectionButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'connection') {

        }
    }


}
