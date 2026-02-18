import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface SelectProps extends BaseFormlyFieldProps, FormlyFieldSelectProps {
    primitive?: boolean;
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
      [valuePrimitive]="props.primitive ?? true"
      [readonly]="props.readonly === true"
      (valueChange)="props.change && props.change(field, $event)"
    >
    </kendo-dropdownlist>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldSelect extends FieldType<FieldTypeConfig<SelectProps>> { }
