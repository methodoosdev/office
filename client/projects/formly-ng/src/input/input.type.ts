import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface InputProps extends BaseFormlyFieldProps { }

export interface FormlyInputFieldConfig extends FormlyFieldConfig<InputProps> {
    type: 'input' | Type<FormlyFieldInput>;
}

@Component({
    selector: 'formly-field-kendo-input',
    template: `
    <input
      *ngIf="props.type !== 'number'; else numberTmp"
      kendoTextBox
      [type]="props.type || 'text'"
      [formlyAttributes]="field"
      [formControl]="formControl"
    />
    <ng-template #numberTmp>
      <kendo-numerictextbox [formlyAttributes]="field" [formControl]="formControl"> </kendo-numerictextbox>
    </ng-template>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldInput extends FieldType<FieldTypeConfig<InputProps>> { }
