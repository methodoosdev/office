import { Component, OnInit } from "@angular/core";
import { GroupDescriptor, groupBy, GroupResult } from "@progress/kendo-data-query";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { IFormlyFormInputs, EmployeeActivityLogUnitOfWork } from "@officeNg";
import { getFieldProperties } from "@formlyNg";
import { FormGroup } from "@angular/forms";

@Component({
    selector: "employee-activity-log",
    templateUrl: "./employee-activity-log.html",
    animations: [stateAnimation]
})
export class EmployeeActivityLogComponent implements OnInit {
    animate: boolean = true;
    gridData: GroupResult[];
    columns: any[];
    toggleValue: boolean = true;

    groups: GroupDescriptor[];
    byActivityLogType: GroupDescriptor[] = [{ field: 'activityLogType' }, { field: 'employeeName' }];
    byTraderName: GroupDescriptor[] = [{ field: 'employeeName' }, { field: 'activityLogType' }];

    title: string;
    calcLabel: string;
    autofitColumnsLabel: string;
    inputs: IFormlyFormInputs;

    constructor(
        private uow: EmployeeActivityLogUnitOfWork,
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
                result.searchModel['from'] = new Date(result.searchModel['from']);
                result.searchModel['to'] = new Date(result.searchModel['to']);

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

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    calc() {
        this.gridData = undefined;

        this.uow.loadDataSource(this.inputs.model)
            .then((data: any) => {
                this.groups = this.toggleValue ? this.byTraderName : this.byActivityLogType;
                this.gridData = groupBy(data, this.groups);

            }).catch((error: Error) => {
                throw error;
            });
    }

    toggle() {
        this.toggleValue = !this.toggleValue;
        if (this.toggleValue) {
            this.columns.find((col) => col.field === 'activityLogType').hidden = false;
            this.columns.find((col) => col.field === 'employeeName').hidden = true;
        } else {
            this.columns.find((col) => col.field === 'employeeName').hidden = false;
            this.columns.find((col) => col.field === 'activityLogType').hidden = true;
        }
        this.calc();
    }

    onVisibilityChange(e: any): void {
        e.columns.forEach((column: any) => {
            this.columns.find((col) => col.field === column.field).hidden = column.hidden;
        });
    }

}
