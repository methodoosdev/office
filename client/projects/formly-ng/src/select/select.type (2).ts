import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface SelectProps extends BaseFormlyFieldProps, FormlyFieldSelectProps {
    primitive?: boolean;
    filterable?: boolean;
    source?: any[];
}

export interface FormlySelectFieldConfig extends FormlyFieldConfig<SelectProps> {
    type: 'select' | Type<FormlyFieldSelect>;
}

@Component({
    selector: 'formly-field-kendo-select',
    template: `
    <kendo-dropdownlist
      [formControl]="formControl"
      [formlyAttributes]="field"
      [data]="props.options | formlySelectOptions: field | async"
      [textField]="'label'"
      [valueField]="'value'"
      [filterable]="props.filterable === true ? true : false"
      [valuePrimitive]="props.primitive ?? true"
      [readonly]="props.readonly === true"
      (filterChange)="onFilterChange(field, $event)"
      (valueChange)="props.change && props.change(field, $event)"
    >
    </kendo-dropdownlist>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldSelect extends FieldType<FieldTypeConfig<SelectProps>> {

    override defaultOptions = {
        hooks: {
            onInit: (field: FormlyFieldConfig) => {
                if (Array.isArray(field.props.options)) {
                    field.props["source"] = field.props.options.slice();
                }
            },
        }
    };

    handleFilter(field: FormlyFieldConfig, value: any) {
        field.props.options = field.props["source"].filter(
            (s) => s.label.toLowerCase().indexOf(value.toLowerCase()) !== -1
        );
    }
}
