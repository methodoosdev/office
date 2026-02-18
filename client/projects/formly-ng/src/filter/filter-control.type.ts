import { Component, ChangeDetectionStrategy, Type, OnInit } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyAttributeEvent, FormlyFieldProps } from '@ngx-formly/core/lib/models';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { ItemArgs } from '@progress/kendo-angular-dropdowns';
import { CompositeFilterDescriptor, filterBy, FilterDescriptor } from '@progress/kendo-data-query';
import { FilterExpression } from '@progress/kendo-angular-filter';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface FilterControlProps extends FormlyFieldProps {
    value: CompositeFilterDescriptor;
}

export interface FormlyFilterControlFieldConfig extends FormlyFieldConfig<FilterControlProps> {
    type: 'multiColumnComboBox' | Type<FormlyFieldFilterControl>;
}

@Component({
    selector: 'formly-field-kendo-multicolumncombobox',
    template: `
        <kendo-filter #filter
            [formControl]="formControl"
            [formlyAttributes]="field"
            [value]="props.value"
            (valueChange)="onFilterChange(field, $event)">
            <ng-container *ngFor="let col of props.columns>
                <ng-container [ngSwitch]="col.fieldType">
                    <ng-container *ngSwitchCase="'date'">
                        <kendo-filter-field field="ordered on" title="Ordered on" editor="date"></kendo-filter-field>
                    </ng-container>
                    <span *ngSwitchCase="'date'">
                        <kendo-filter-field field="discontinued" title="Discontinued" editor="boolean"></kendo-filter-field>
                    </span>
                    <span *ngSwitchCase="'decimal'">
                        <kendo-filter-field field="discontinued" title="Discontinued" editor="boolean"></kendo-filter-field>
                    </span>
                    <span *ngSwitchDefault>
                        <kendo-filter-field field="discontinued" title="Discontinued" editor="boolean"></kendo-filter-field>
                    </span>
                </ng-container>
            </ng-container>
        </kendo-filter>
    `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldFilterControl extends FieldType<FieldTypeConfig<FilterControlProps>> {


    value: CompositeFilterDescriptor = {
        logic: 'or', filters: [
            { field: 'budget', operator: 'gt' },
            { field: 'country', operator: 'contains' },
            { field: 'discontinued', operator: 'eq', value: true },
            { logic: 'and', filters: [{ field: 'ordered on', operator: 'gt', value: new Date(Date.now()) }] }]
    };

    filters: FilterExpression[] = [
        {
            field: 'country',
            title: 'Country',
            editor: 'string',
            operators: ['neq', 'eq', 'contains']
        },
        {
            field: 'budget',
            editor: 'number'
        },
        {
            field: 'discontinued',
            title: 'Discontinued',
            editor: 'boolean'
        },
        {
            field: 'ordered on',
            title: 'Ordered on',
            editor: 'date'
        }
    ];

    onFilterChange(field: FormlyFieldConfig, value: CompositeFilterDescriptor): void {
        //console.log(value);
    }

    editorValueChange(value, currentItem: FilterDescriptor, filterValue: CompositeFilterDescriptor): void {
        currentItem.value = value;
        //console.log(filterValue);
    }
}
