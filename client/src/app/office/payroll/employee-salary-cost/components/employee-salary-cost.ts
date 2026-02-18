import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { EmployeeSalaryCostUnitOfWork } from '@officeNg';
import { TranslationService } from "@core";
import { getFieldProperties } from "@formlyNg";

@Component({
    selector: "employee-salary-cost",
    templateUrl: "./employee-salary-cost.html",
    encapsulation: ViewEncapsulation.None,
    styles: [`
        .text-bold input.k-input-inner {
            font-weight: bold;
        }
    `],
    animations: [stateAnimation]
})

export class EmployeeSalaryCostComponent implements OnInit {
    animate: boolean = true;
    title: string;
    searchForm: any;
    searchFormCustomProperties: any;
    dataFormFields: any;
    dataForm: any;
    dataFormCustomProperties: any;

    dataResultFormFields: any;
    dataResultForm: any;
    panelLabel: string;
    calcLabel: string;
    packages: any[] = [];
    
    constructor(
        private uow: EmployeeSalaryCostUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
            
        this.calcLabel = this.translationService.translate('common.calc');
    }

    modelChangeEvent(value: any) {
        this.dataForm = undefined;
    }

    async getPackages(model: any) {
        return await this.uow.getPackages(model);
    }

    ngOnInit(): void {

        this.uow.loadProperties()
            .then((result: any) => {

                const { fields, ...customProperties } = result.searchFormModel.customProperties;
                const properties = getFieldProperties(fields);

                this.title = customProperties.title;
                this.panelLabel = `${this.title} - ${this.calcLabel}`;
                this.searchFormCustomProperties = customProperties;
                this.dataFormFields = result.dataFormModel.customProperties.fields;
                this.dataResultFormFields = result.dataResultFormModel.customProperties.fields;

                properties['insurancePackageId'].props['updateOptions'] = (newOptions: any[]) => {

                    properties['insurancePackageId'].props['options'] = newOptions;
                    properties['insurancePackageId'].props['__source'] = newOptions;
                };

                if (properties['companyId']) {
                    properties['companyId'].props['selectionChange'] = (field: FormlyFieldConfig, employers: any) => {

                        if (employers?.value > 0) {

                            setTimeout(() => {
                                this.getPackages(field.model)
                                    .then((result: any) => {
                                        field.form.get("insurancePackageId").setValue(null);
                                        properties['insurancePackageId'].props['updateOptions'](result.data);
                                    }).catch((err) => {
                                        throw err;
                                    });
                            });

                        } else {
                            field.form.get("insurancePackageId").setValue(null);
                            properties['insurancePackageId'].props['updateOptions'](result.data);

                        }
                    };
                }

                properties['allPackages'].props['change'] = (field: FormlyFieldConfig, value: boolean) => {

                    setTimeout(() => {
                        this.uow.getPackages(field.model)
                            .then((result: any) => {
                                properties['insurancePackageId'].props['updateOptions'](result.data);
                            }).catch((err) => {
                                throw err;
                            });
                    });
                };

                this.searchForm = {
                    form: new FormGroup({}),
                    options: {
                        formState: {
                            getPackages: this.getPackages,
                            packages: this.packages,
                        },
                    },
                    fields: fields,
                    model: result.searchModel
                };


            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    get canCalc() {
        return this.searchForm
            && this.searchForm.model
            && this.searchForm.model.companyId > 0
            && this.searchForm.model.insurancePackageId > 0;
    }

    calc() {
        this.dataForm = undefined;

        this.uow.loadDataSource(this.searchForm.model)
            .then((result: any) => {

                this.dataResultForm = {
                    form: new FormGroup({}),
                    options: {},
                    fields: this.dataResultFormFields,
                    model: result.model
                };

                this.dataForm = {
                    form: new FormGroup({}),
                    options: {},
                    fields: this.dataFormFields,
                    model: result.model
                };

            }).catch((err) => {               
                throw err;
            });
    }
}

