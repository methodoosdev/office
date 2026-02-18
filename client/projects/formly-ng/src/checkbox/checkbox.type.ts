import { Component, ChangeDetectionStrategy, ViewEncapsulation, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { BaseFormlyFieldProps, FieldType } from '../form-field/public_api';

interface CheckboxProps extends BaseFormlyFieldProps { }

export interface FormlyCheckboxFieldConfig extends FormlyFieldConfig<CheckboxProps> {
    type: 'checkbox' | Type<FormlyFieldCheckbox>;
}

@Component({
    selector: 'formly-field-kendo-checkbox',
    template: `
    <input type="checkbox" kendoCheckBox [formControl]="formControl" [formlyAttributes]="field" />
    <label *ngIf="props.label && props.hideLabel !== false" [for]="id" class="k-checkbox-label">
      {{ props.label }}
      <span *ngIf="props.required && props.hideRequiredMarker !== true" aria-hidden="true" class="k-required">*</span>
    </label>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./checkbox.type.scss'],
})
export class FormlyFieldCheckbox extends FieldType<FieldTypeConfig<CheckboxProps>> {
    override defaultOptions = {
        props: {
            hideLabel: false,
        },
    };
}
