import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface TextProps extends BaseFormlyFieldProps { }

export interface FormlyTextFieldConfig extends FormlyFieldConfig<TextProps> {
    type: 'text' | Type<FormlyFieldText>;
}

@Component({
    selector: 'formly-field-kendo-text',
    template: ` 
    <input kendoTextBox [type]="props.type || 'text'" [formlyAttributes]="field" [formControl]="formControl" />`,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldText extends FieldType<FieldTypeConfig<TextProps>> { }
