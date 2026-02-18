import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface TextButtonProps extends BaseFormlyFieldProps {
    onClick?: (field: FormlyFieldConfig, event: any) => void;
}

export interface FormlyTextButtonFieldConfig extends FormlyFieldConfig<TextButtonProps> {
    type: 'textButton' | Type<FormlyFieldTextButton>;
}

@Component({
    selector: 'formly-field-kendo-text-button',
    template: `
    <kendo-textbox [formControl]="formControl" [formlyAttributes]="field" [readonly]="props.readonly">
        <ng-template kendoTextBoxSuffixTemplate *ngIf="props.readonly !== true">
            <button kendoButton fillMode="flat" (click)="props.onClick && props.onClick(field, $event)"
                [disabled]="props.disabled" icon="hyperlink-open">
            </button>
        </ng-template>
    </kendo-textbox>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldTextButton extends FieldType<FieldTypeConfig<TextButtonProps>> { }
