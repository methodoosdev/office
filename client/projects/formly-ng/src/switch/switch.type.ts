import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { BaseFormlyFieldProps, FieldType } from '../form-field/public_api';
import { PrimeNGConfig } from '@primeNg';

interface SwitchProps extends BaseFormlyFieldProps {
    style?: string;
    onLabel?: string;
    offLabel?: string;
}

export interface FormlySwitchFieldConfig extends FormlyFieldConfig<SwitchProps> {
    type: 'switch' | Type<FormlyFieldSwitch>;
}

@Component({
    selector: 'formly-field-kendo-switch',
    template: `
    <kendo-switch
        [formControl]="formControl"
        [formlyAttributes]="field"
        [readonly]="props.readonly === true"
        [style]="props.style || style"
        [onLabel]="props.onLabel || onLabel"
        [offLabel]="props.offLabel || offLabel">
      </kendo-switch>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormlyFieldSwitch extends FieldType<FieldTypeConfig<SwitchProps>> {
    style: string;
    onLabel: string;
    offLabel: string;

    constructor(config: PrimeNGConfig) {
        super();
        this.style = "width: 60px;"
        this.onLabel = config.getTranslation("common.yes");
        this.offLabel = config.getTranslation("common.no");
    }
}
