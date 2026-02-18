import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { LabelSettings } from '@progress/kendo-angular-progressbar';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface ProgressBarProps extends BaseFormlyFieldProps {
    value?: number;
}

export interface FormlyProgressBarFieldConfig extends FormlyFieldConfig<ProgressBarProps> {
    type: 'progressBar' | Type<FormlyFieldProgressBar>;
}

@Component({
    selector: 'formly-field-kendo-progressbar',
    template: `
    <kendo-progressbar
        [formlyAttributes]="field"
        [label]="progressLabel"
        [value]="props.value">
    </kendo-progressbar>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormlyFieldProgressBar extends FieldType<FieldTypeConfig<ProgressBarProps>> {

    progressLabel: LabelSettings = {
        visible: true,
        format: "percent",
        position: "center",
    };
}
