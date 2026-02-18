import { Component, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";

import { ApiConfigService, TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { GridViewToken, IFormlyFormInputs, SoftoneProjectUnitOfWork } from "@officeNg";
import { getFieldProperties } from "@formlyNg";

@Component({
    selector: "softone-project",
    templateUrl: "./softone-project.html",
    animations: [stateAnimation]
})
export class SoftoneProjectComponent implements OnInit {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    title: string;
    pathUrl = 'office/softone-project';

    infoInputs: IFormlyFormInputs;
    infoModel: any;

    constructor(
        public uow: SoftoneProjectUnitOfWork,
        private translationService: TranslationService) {

        this.title = this.translationService.translate('menu.projects')
    }
    refresh() { }
    modelChangeEvent(value: any) {
        this.table?.clearData();

        if (value["traderId"] && value["traderId"] > 0) {
            this.infoModel = value;
            setTimeout(() => {
                this.table.loadDataSource();
            });
        }
    }

    loadPropertiesEvent(result: any) {

        if (!(result.infoModel.traderId > 0)) {
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
        } else {
            this.infoModel = result.infoModel;
            setTimeout(() => {
                this.table.loadDataSource();
            });
        }

    }

    ngOnInit(): void {

        //this.uow.loadProperties()
        //    .then((result: any) => {

        //        this.title = result.searchModel.title;

        //        const { fields, ...customPropertiesRest } = result.infoFormModel.customProperties;
        //        const properties = getFieldProperties(fields);

        //        const infoInputs = {
        //            form: new FormGroup({}),
        //            options: {},
        //            fields: fields,
        //            model: result.infoModel,
        //            properties: properties,
        //            customProperties: customPropertiesRest
        //        };
        //        this.infoInputs = infoInputs;

        //    }).catch((error: Error) => {
        //        throw error;
        //    });
    }

    //ngOnInit2(): void {
    //    const trader = this.api.configuration.trader;

    //    if (!!trader) {
    //        this.traderId = trader.id;
    //        this.userIsTrader = true;

    //    } else {
    //        this.uow.getPreview()
    //            .then((result: any) => {
    //                this.previewFields = result.previewModel.customProperties.fields;
    //                this.previewModel = result.previewModel;

    //                const fieldProperties = getFieldProperties(this.previewFields);

    //                fieldProperties['traderId'].props['selectionChange'] = (field: FormlyFieldConfig, trader: any) => {

    //                    this.traderId = trader?.id;
    //                };

    //            }).catch((error: Error) => {
    //                throw error;
    //            });
    //    }
    //}
}
