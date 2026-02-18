import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface AutocompleteProps extends BaseFormlyFieldProps, FormlyFieldSelectProps {
    data?: any[];
}

export interface FormlyAutocompleteFieldConfig extends FormlyFieldConfig<AutocompleteProps> {
    type: 'autocomplete' | Type<FormlyFieldAutocomplete>;
}

@Component({
    selector: 'formly-field-kendo-autocomplete',
    template: `
    <kendo-autocomplete
      [formControl]="formControl"
      [formlyAttributes]="field"
      [data]="props.options | formlySelectOptions: field | async"
      [valueField]="'value'"
      [filterable]="true"
      (filterChange)="handleFilter($event,field)"
      (valueChange)="props.change && props.change(field, $event)"
    >
    </kendo-autocomplete>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldAutocomplete extends FieldType<FieldTypeConfig<AutocompleteProps>> {

    override defaultOptions = {
        hooks: {
            onInit: (field: FormlyFieldConfig) => {
                field.props["data"] = (field.props.options as any[]).slice();
            },
        }
    };

    handleFilter(value: string, field: FormlyFieldConfig) {
        field.props.options = field.props["data"].filter(
            (s) => s.value.toLowerCase().indexOf(value.toLowerCase()) !== -1
        );
    }
}
