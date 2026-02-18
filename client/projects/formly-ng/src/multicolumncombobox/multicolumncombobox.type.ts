import { Component, ChangeDetectionStrategy, Type, OnInit } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyAttributeEvent } from '@ngx-formly/core/lib/models';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { ItemArgs } from '@progress/kendo-angular-dropdowns';
import { filterBy, FilterDescriptor } from '@progress/kendo-data-query';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface MultiColumnComboBoxProps extends BaseFormlyFieldProps, FormlyFieldSelectProps {
    listHeight?: number;
    columns: any[];
    selectionChange?: FormlyAttributeEvent;
}

export interface FormlyMultiColumnComboBoxFieldConfig extends FormlyFieldConfig<MultiColumnComboBoxProps> {
    type: 'multiColumnComboBox' | Type<FormlyFieldMultiColumnComboBox>;
}

@Component({
    selector: 'formly-field-kendo-multicolumncombobox',
    template: `
    <kendo-multicolumncombobox
        [formControl]="formControl"
        [formlyAttributes]="field"
        [data]="props.options"
        [textField]="props.labelProp?.toString() || 'label'"
        [valueField]="props.valueProp?.toString() || 'value'"
        [valuePrimitive]="true"
        [readonly]="props.readonly === true"
        [placeholder]="props.placeholder"
        [itemDisabled]="itemDisabled"
        [filterable]="true"
        (filterChange)="handleFilterChange(field, $event)"
        (selectionChange)="props.selectionChange && props.selectionChange(field, $event)">
            <kendo-combobox-column *ngFor="let col of props.columns"
                [width]="col.width"
                [field]="col.field"
                [title]="col.title"
                [style]="col.style"
                [headerStyle]="col.headerStyle"
            >
                <ng-template kendoMultiColumnComboBoxColumnCellTemplate let-dataItem>
                    <ng-container [ngSwitch]="col.fieldType">
                        <span *ngSwitchCase="'checkbox'" class="checkbox-column">
                            <span class="k-icon k-icon-lg" [ngClass]="{'k-i-minus-outline k-color-error': !dataItem[col.field], 'k-i-check-circle k-color-success': dataItem[col.field]}"></span>
                        </span>
                        <span *ngSwitchCase="'date'" class="date-column">
                            {{dataItem[col.field] | kendoDate: "d"}}
                        </span>
                        <span *ngSwitchCase="'decimal'" class="decimal-column">
                            {{dataItem[col.field] | kendoNumber: '#,##0.00'}}
                        </span>
                        <span *ngSwitchDefault>
                            {{dataItem[col.field]}}
                        </span>
                    </ng-container>
                </ng-template>
            </kendo-combobox-column>
    </kendo-multicolumncombobox>
  `,
    styles: [`
    kendo-multicolumncombobox {
        width: 100%;
        }
    `],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldMultiColumnComboBox extends FieldType<FieldTypeConfig<MultiColumnComboBoxProps>> implements OnInit {
    source: any[];

    itemDisabled: (args: ItemArgs) => boolean;

    _itemDisabled(args: ItemArgs): boolean {
        const item: any = args.dataItem;
        const prop = this.field.props.disabledProp as string;

        return !item[prop];
    }

    ngOnInit(): void {
        this.source = (this.field.props.options as any[]).slice();
        this.itemDisabled = this._itemDisabled.bind(this);
    }

    handleFilterChange(field: FormlyFieldConfig, searchTerm: string): void {
        const filters: FilterDescriptor[] = [];
        const sourceModel = this.source[0];

        if (sourceModel) {
            Object.keys(sourceModel).forEach((key) => {
                if (typeof sourceModel[key] === "string") {
                    filters.push({ field: key, operator: "contains", value: searchTerm, ignoreCase: true });
                }
            });
        }

        field.props.options = filterBy(this.source, {
            logic: 'or',
            filters: filters
        });
    }

}
