import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyAttributeEvent } from '@ngx-formly/core/lib/models';
import { CalendarView } from '@progress/kendo-angular-dateinputs';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface DateProps extends BaseFormlyFieldProps {
    format?: string;
    activeView?: CalendarView;
    bottomView?: CalendarView;
    minDate?: Date;
    maxDate?: Date;
    nullable?: boolean;
    valueChange?: FormlyAttributeEvent;
}

export interface FormlyDateFieldConfig extends FormlyFieldConfig<DateProps> {
    type: 'date' | Type<FormlyFieldDate>;
}

@Component({
    selector: "formly-field-kendo-date",
    template: `
    <kendo-datepicker 
        [formControl]="formControl" 
        [formlyAttributes]="field"
        [format]="props.format" 
        [min]="props.minDate" [max]="props.maxDate"
        [activeView]="props.activeView" 
        [bottomView]="props.bottomView" 
        [readonly]="props.readonly === true"
        (valueChange)="onChange($event, field)">
    </kendo-datepicker>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormlyFieldDate extends FieldType<FieldTypeConfig<DateProps>> {
    _localTimeRegex = /.\d{3}$/;

    parseDateAsUTC(source: any) {
        if (typeof source === 'string') {
            // convert to UTC string if no time zone specifier.
            var isLocalTime = this._localTimeRegex.test(source);
            // var isLocalTime = !hasTimeZone(source);
            source = isLocalTime ? source + 'Z' : source;
        }
        source = new Date(Date.parse(source));
        return source;
    };

    parseDateWithoutTimezone(val: string): Date {

        const date = Date.parse(val);
        if (!isNaN(date)) {
            const value = new Date(date);
            const userTimezoneOffset = value.getTimezoneOffset() * 60000;

            return new Date(value.getTime() - userTimezoneOffset);
        }
        return null;
    }
    parseDate(val: string): Date {

        const date = Date.parse(val);
        if (!isNaN(date)) {
            const value = new Date(date).toUTCString();
            return new Date(value);
        }
        return null;
    }
    override defaultOptions = {
        hooks: {
            onInit: (field: FormlyFieldConfig) => {
                const value = field.formControl.value as string;
                //const value = this.parseDateAsUTC(field.formControl.value);
                //const value = this.parseDateWithoutTimezone(field.formControl.value);
                const key = field.key as string;
                const props = field.props as DateProps;

                if (value) {
                    const date = Date.parse(value);
                    if (!isNaN(date)) {
                        field.model[key] = new Date(date);
                        field.options.resetModel(field.model);
                    }
                }
            },
            _afterViewInit: (field: FormlyFieldConfig) => {

                setTimeout(() => {
                    const props = field.props as DateProps;

                    const minDate = props.minDate;
                    if (minDate) {
                        const date = Date.parse(minDate.toString());
                        if (!isNaN(date)) {
                            props.minDate = new Date(date);
                        }
                    }
                    const maxDate = props.maxDate;
                    if (maxDate) {
                        const date = Date.parse(maxDate.toString());
                        if (!isNaN(date)) {
                            props.maxDate = new Date(date);
                        }
                    }
                });
            }
        }
    };

    onChange(value: Date, field: FormlyFieldConfig) {
        const props = this.field.props as DateProps;

        // value not be null
        if (props.nullable === false) {
            const date = Date.parse(value?.toString());
            if (isNaN(date)) {
                const val = props.minDate ? props.minDate : (props.maxDate ? props.maxDate : new Date());
                this.field.formControl.setValue(val);
            }
        }

        props.valueChange && props.valueChange(field, value)
    }
}
