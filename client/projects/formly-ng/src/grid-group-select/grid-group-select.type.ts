import { Component, ChangeDetectionStrategy, Type, ViewEncapsulation } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyAttributeEvent } from '@ngx-formly/core/lib/models';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';
import { GroupResult, groupBy } from '@progress/kendo-data-query';

interface GridGroupSelectProps extends BaseFormlyFieldProps, FormlyFieldSelectProps {
    listHeight?: number;
    groupable?: boolean;
    selectionChange?: FormlyAttributeEvent;
    sourceOptions?: any[] | GroupResult[];
}

export interface FormlyGridGroupSelectFieldConfig extends FormlyFieldConfig<GridGroupSelectProps> {
    type: 'gridGroupSelect' | Type<FormlyFieldGridGroupSelect>;
}

@Component({
    selector: 'formly-field-kendo-grid-group-select',
    template: `
        <kendo-multicolumncombobox
            [formControl]="formControl"
            [formlyAttributes]="field"
            [showStickyHeader]="props.groupable === true"
            [listHeight]="props.listHeight"
            [data]="props.options"
            [textField]="'label'"
            [valueField]="'value'"
            [readonly]="props.readonly === true"
            [valuePrimitive]="true"
            (valueChange)="props.change && props.change(field, $event)"
            [placeholder]="props.placeholder"
            [style]="{'width': '100%'}"
            (selectionChange)="props.selectionChange && props.selectionChange(field, $event)"
            [filterable]="true"
            (filterChange)="handleFilterChange($event, field)">
            <kendo-combobox-column [field]="'label'" [headerStyle]="{'display':'none'}"></kendo-combobox-column>
            <kendo-combobox-column *ngIf="props.groupable === true" [field]="$any(props.groupProp)" [headerStyle]="{'display':'none'}">
            </kendo-combobox-column>
        </kendo-multicolumncombobox>
  `,
    styles: [
        `
            .k-table-group-sticky-header .k-table-th {
                font-weight: 600;
            }
        `
    ],
    encapsulation: ViewEncapsulation.None,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldGridGroupSelect extends FieldType<FieldTypeConfig<GridGroupSelectProps>> {

    override defaultOptions = {
        hooks: {
            onInit: (field: FormlyFieldConfig<GridGroupSelectProps>) => {
                const source = field.props.sourceOptions.slice();
                const group = field.props.groupProp as string;

                if (field.props.groupable === true)
                    field.props.options = groupBy(source, [{ field: group }]);
                else
                    field.props.options = source;
            }
        }
    };

    public handleFilterChange(searchTerm: string, field: FormlyFieldConfig<GridGroupSelectProps>): void {
        const normalizedQuery = searchTerm.toLowerCase();

        // search in all three fields diplayed in the popup table
        const filterExpression = (dataItem: any) => dataItem["label"].toLowerCase().includes(normalizedQuery);

        const source = field.props.sourceOptions.filter(filterExpression);
        const group = field.props.groupProp as string;

        if (field.props.groupable === true)
            field.props.options = groupBy(source, [{ field: group }]);
        else
            field.props.options = source;
    }
}
