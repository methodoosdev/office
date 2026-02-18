import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface TextAreaProps extends BaseFormlyFieldProps { }

export interface FormlyTextAreaFieldConfig extends FormlyFieldConfig<TextAreaProps> {
    type: 'textarea' | Type<FormlyFieldTextArea>;
}

@Component({
    selector: 'formly-field-kendo-textarea',
    template: ` <textarea kendoTextArea [rows]="props.rows" [formControl]="formControl" [formlyAttributes]="field"></textarea> `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldTextArea extends FieldType<FieldTypeConfig<TextAreaProps>> { }
