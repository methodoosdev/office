import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { InputRounded, InputSize } from '@progress/kendo-angular-inputs';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface NumericProps extends BaseFormlyFieldProps {
    decimals?: number;
    format?: string;
    size?: InputSize;
    rounded?: InputRounded;
    spinners?: boolean;
}

export interface FormlyNumericFieldConfig extends FormlyFieldConfig<NumericProps> {
    type: 'numeric' | Type<FormlyFieldNumeric>;
}

@Component({
    selector: 'formly-field-kendo-numeric',
    template: `
    <kendo-numerictextbox         
        [decimals]="props.decimals" [format]="props.format"
        [readonly]="props.readonly === true"
        [size]="props.size" [rounded]="props.rounded" [spinners]="props.spinners"
        [formlyAttributes]="field" [formControl]="formControl"
        (valueChange)="onValueChange($event)"> 
    </kendo-numerictextbox>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldNumeric extends FieldType<FieldTypeConfig<NumericProps>> {
    onValueChange(value: any) {
        if (value === null)
            this.field.formControl.setValue(0);
    }
}
