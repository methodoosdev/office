import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface TimeProps extends BaseFormlyFieldProps {
    format?: string;
    minDate?: Date;
    maxDate?: Date;
    nullable?: boolean;
}

export interface FormlyTimeFieldConfig extends FormlyFieldConfig<TimeProps> {
    type: 'time' | Type<FormlyFieldTime>;
}

@Component({
    selector: "formly-field-kendo-time",
    template: `
    <kendo-timepicker [formControl]="formControl" 
                      [formlyAttributes]="field"
                      [format]="props.format" 
                      [min]="minDate" [max]="maxDate" 
                      [nowButton]="false"
                      [readonly]="props.readonly === true"
                      (valueChange)="onChange($event)">
    </kendo-timepicker>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormlyFieldTime extends FieldType<FieldTypeConfig<TimeProps>> {
    minDate: Date;
    maxDate: Date;
    defaultDate = new Date(2000, 2, 2, 0, 0, 0); // 1 is Feb

    override defaultOptions = {
        hooks: {
            onInit: (field: FormlyFieldConfig) => {
                const value = field.formControl.value as string;
                const key = field.key as string;
                const props = field.props as TimeProps;

                if (value) {
                    const date = Date.parse(value);
                    if (!isNaN(date)) {
                        field.model[key] = new Date(date);
                        field.options.resetModel(field.model);
                    }
                }

                const minDate = props.minDate;
                if (minDate) {
                    const date = Date.parse(minDate.toString());
                    if (!isNaN(date)) {
                        this.minDate = new Date(date);
                    }
                }
                const maxDate = props.maxDate;
                if (maxDate) {
                    const date = Date.parse(maxDate.toString());
                    if (!isNaN(date)) {
                        this.maxDate = new Date(date);
                    }
                }
            }
        }
    };

    onChange(value: Date) {
        const props = this.field.props as TimeProps;

        // value not be null
        if (props.nullable === false) {
            const date = Date.parse(value?.toString());
            if (isNaN(date)) {
                const val = this.minDate ? this.minDate : (this.maxDate ? this.maxDate : this.defaultDate);
                this.field.formControl.setValue(val);
            }
        }
    }
}
