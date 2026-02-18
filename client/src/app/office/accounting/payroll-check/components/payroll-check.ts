import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { DomSanitizer, SafeStyle } from "@angular/platform-browser";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { IFormlyFormInputs, PayrollCheckUnitOfWork } from "@officeNg";
import { getFieldProperties } from "@formlyNg";
import { FormlyFieldConfig } from "@ngx-formly/core";

@Component({
    selector: "payroll-check",
    templateUrl: "./payroll-check.html",
    animations: [stateAnimation]
})
export class PayrollCheckComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    calcLabel: string;
    autofitColumnsLabel: string;
    inputs: IFormlyFormInputs;
    
    constructor(
        private uow: PayrollCheckUnitOfWork,
        private sanitizer: DomSanitizer,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
        this.calcLabel = this.translationService.translate('common.calc');
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    ngOnInit(): void {
        this.uow.loadProperties()
            .then((result: any) => {
                result.searchModel['periodos'] = new Date(result.searchModel['periodos']);

                this.title = result.tableModel.customProperties.title;
                this.columns = result.tableModel.customProperties.columns;

                const { fields, ...customPropertiesRest } = result.formModel.customProperties;
                const properties = getFieldProperties(fields);

                const inputs = {
                    form: new FormGroup({}),
                    options: {},
                    fields: fields,
                    model: result.searchModel,
                    properties: properties,
                    customProperties: customPropertiesRest
                };
                this.inputs = inputs;
                
                properties['employeeId'].props['selectionChange'] = (field: FormlyFieldConfig, model: any) => {
                    
                    if (model) {
                        const employee = (field.props.options as any[]).find(i => i.value === model.value);
                        field.form.get('employerIds').setValue(employee.ids);
                    } else {
                        field.form.get('employerIds').setValue([]);
                    }
                };

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        return this.inputs && this.inputs.model && this.inputs.model.employerIds.length > 0;
    }

    calc() {
        this.gridData = undefined;

        this.uow.loadDataSource(this.inputs.model)
            .then((data: any[]) => {
                this.gridData = data;

            }).catch((error: Error) => {
                throw error;
            });
    }

    public colorCode(dataItem: any, field: string): SafeStyle {
        const value = dataItem[`_${field}`] === true;
        const result = !value ? '#424242' : '#ff0000';
        return this.sanitizer.bypassSecurityTrustStyle(result);
    }

}
