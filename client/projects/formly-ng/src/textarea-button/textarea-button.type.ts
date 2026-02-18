import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface TextareaButtonProps extends BaseFormlyFieldProps {
    onClick?: (field: FormlyFieldConfig, event: any) => void;
}

export interface FormlyTextareaButtonFieldConfig extends FormlyFieldConfig<TextareaButtonProps> {
    type: 'textareaButton' | Type<FormlyFieldTextareaButton>;
}

@Component({
    selector: 'formly-field-kendo-textarea-button',
    template: `
    <kendo-textarea
        [formControl]="formControl"
        [formlyAttributes]="field"
        [readonly]="props.readonly"
        flow="horizontal">
          <kendo-textarea-suffix *ngIf="props.readonly !== true">
            <button kendoButton fillMode="flat" (click)="props.onClick && props.onClick(field, $event)"
                [disabled]="props.disabled" icon="hyperlink-open">
            </button>
          </kendo-textarea-suffix>
        </kendo-textarea>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldTextareaButton extends FieldType<FieldTypeConfig<TextareaButtonProps>> { }
