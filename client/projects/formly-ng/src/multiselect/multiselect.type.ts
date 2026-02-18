import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface MultiSelectProps extends BaseFormlyFieldProps, FormlyFieldSelectProps { }

export interface FormlyMultiSelectFieldConfig extends FormlyFieldConfig<MultiSelectProps> {
    type: 'multiselect' | Type<FormlyFieldMultiSelect>;
}

@Component({
    selector: 'formly-field-kendo-multiselect',
    template: `
    <kendo-multiselect
      [formControl]="formControl"
      [formlyAttributes]="field"
      [data]="props.options | formlySelectOptions: field | async"
      [textField]="'label'"
      [valueField]="'value'"
      [valuePrimitive]="true"
      [readonly]="props.readonly === true"
      (valueChange)="props.change && props.change(field, $event)"
    >
    </kendo-multiselect>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldMultiSelect extends FieldType<FieldTypeConfig<MultiSelectProps>> { }
