import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { ButtonThemeColor } from '@progress/kendo-angular-buttons';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface ButtonProps extends BaseFormlyFieldProps {
    themeColor?: ButtonThemeColor;
}

export interface FormlyButtonFieldConfig extends FormlyFieldConfig<ButtonProps> {
    type: 'button' | Type<FormlyFieldButton>;
}

@Component({
    selector: 'formly-field-kendo-button',
    template: `
    <button kendoButton
        [disabled]="props.disabled"
        (click)="props.click && props.click(field, $event)"
        [themeColor]="props.themeColor">
            {{ props.label }}
    </button>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldButton extends FieldType<FieldTypeConfig<ButtonProps>> { }
