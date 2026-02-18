import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyAttributeEvent } from '@ngx-formly/core/lib/models';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface GridSelectProps extends BaseFormlyFieldProps, FormlyFieldSelectProps {
    listHeight?: number;
    selectionChange?: FormlyAttributeEvent;
    updateOptions?: (newOptions: any[]) => void;
    __source?: any[];
}

export interface FormlyGridSelectFieldConfig extends FormlyFieldConfig<GridSelectProps> {
    type: 'gridSelect' | Type<FormlyFieldGridSelect>;
}

@Component({
    selector: 'formly-field-kendo-grid-select',
    template: `
    <kendo-multicolumncombobox
      [formControl]="formControl"
      [formlyAttributes]="field"
      [data]="props.options | formlySelectOptions: field | async"
      [textField]="'label'"
      [valueField]="'value'"
      [valuePrimitive]="true"
      [readonly]="props.readonly === true"
      (valueChange)="props.change && props.change(field, $event)"
      [placeholder]="props.placeholder"
      [style]="{'width': '100%'}"
      (selectionChange)="props.selectionChange && props.selectionChange(field, $event)"
      [filterable]="true"
      (filterChange)="handleFilterChange($event, field)">
        <kendo-combobox-column
            [field]="'label'" 
            [headerStyle]="{'display':'none'}">
        </kendo-combobox-column>
    </kendo-multicolumncombobox>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldGridSelect extends FieldType<FieldTypeConfig<GridSelectProps>> {

    override defaultOptions = {
        hooks: {
            onInit: (field: FormlyFieldConfig<GridSelectProps>) => {
                field.props.__source = [...(field.props.options as any[])];
            }
        }
    };
    
    handleFilterChange(search: string, field: FormlyFieldConfig<GridSelectProps>): void {
        const value = search.toLowerCase();
        const labelProp: string = ((field.props as GridSelectProps).labelProp) as string;

        if (typeof labelProp === "string") {
            // search in all three fields diplayed in the popup table
            const filterExpression = (dataItem: any) => dataItem[labelProp].toLowerCase().includes(value);

            field.props.options = field.props.__source.filter(filterExpression);
        }
    }

    onUpdateOptions(newOptions: any[]): void {
        this.field.props.options = [...newOptions];        
        this.field.props.__source = [...newOptions];
    }

    //onSelectionChange(value: any): void {
    //    FieldType.onFireEvent.next({ field: this.field.key, dataItem: value });
    //}
}
