import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';
import { PrimeNGConfig } from '@primeNg';

interface MultiSelectAllProps extends BaseFormlyFieldProps, FormlyFieldSelectProps { }

export interface FormlyMultiSelectAllFieldConfig extends FormlyFieldConfig<MultiSelectAllProps> {
    type: 'multiSelectAll' | Type<FormlyFieldMultiSelectAll>;
}

@Component({
    selector: 'formly-field-kendo-multiselectall',
    template: `
    <kendo-multiselect
        [formControl]="formControl"
        [formlyAttributes]="field"
        [data]="props.options | formlySelectOptions: field | async"
        [textField]="props.labelProp?.toString() || 'label'"
        [valueField]="props.valueProp?.toString() || 'value'"
        [valuePrimitive]="true"
        [readonly]="props.readonly === true"
        [autoClose]="false"
        [tagMapper]="tagMapper"
        [kendoDropDownFilter]="filterSettings"
        (valueChange)="onValueChange(field, $event)"
    >
    <ng-template kendoMultiSelectHeaderTemplate>
        <span style="margin: 8px;">
            <input type="checkbox" id="chk" kendoCheckBox [checked]="isChecked"
                [indeterminate]="isIndet" (click)="onClick()" />
            <kendo-label for="chk" style="margin-left: 8px;">{{ checkAll }}</kendo-label>
        </span>
    </ng-template>

    <ng-template kendoMultiSelectItemTemplate let-dataItem>
        <span style="white-space: nowrap;margin-left: 1rem;">
            <kendo-label for="chk-{{ dataItem[props.valueProp?.toString() || 'value'] }}">
                <input type="checkbox" id="chk-{{ dataItem[props.valueProp?.toString() || 'value'] }}"
                    kendoCheckBox [checked]="isItemSelected(dataItem)" />
            </kendo-label>
        </span>
        <span class="k-list-item-text" style="margin-left: 8px;">{{ dataItem[props.labelProp?.toString() || 'label'] }}</span>
    </ng-template>

    <ng-template kendoMultiSelectGroupTagTemplate let-dataItem>
        {{ dataItem.length }} {{ itemsSelected }}
    </ng-template>

    </kendo-multiselect>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldMultiSelectAll extends FieldType<FieldTypeConfig<MultiSelectAllProps>> {
    checkAll: string;
    itemsSelected: string;

    isChecked = false;

    filterSettings: DropDownFilterSettings = {
        caseSensitive: false,
        operator: "contains"
    };

    constructor(config: PrimeNGConfig) {
        super();
        this.checkAll = config.getTranslation("common.checkAll");
        this.itemsSelected = config.getTranslation("common.itemsSelected");
    }

    tagMapper(tags: any[]): any[] {
        return tags.length < 3 ? tags : [tags];
    }

    get isIndet() {
        const key = this.field.key as string;
        const values = this.field.form.get(key).value as any[];
        const options = this.field.props.options as any[];

        return (
            values.length !== 0 && values.length !== options.length
        );
    }

    isItemSelected(item) {
        const key = this.field.key as string;
        const values = this.field.form.get(key).value as any[];
        const valueProp = this.field.props.valueProp?.toString() || 'value';

        return values.some((x) => {
            return x === item[valueProp];
        });
    }

    onClick() {
        const key = this.field.key as string;
        const options = this.field.props.options as any[];
        const valueProp = this.field.props.valueProp?.toString() || 'value';
        const isChecked = !this.isChecked;
        const values = isChecked ? options.map(item => item[valueProp]) : [];

        this.isChecked = isChecked;
        this.field.form.get(key).setValue(values);
    }

    onValueChange(field: FormlyFieldConfig, values: any[]) {
        const options = field.props.options as any[];

        this.isChecked = values.length === options.length;
        field.props.change && field.props.change(field, values);
    }
}
