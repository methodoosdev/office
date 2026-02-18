import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyFieldSelectProps } from '@ngx-formly/core/select';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface MultiSelectTreeProps extends BaseFormlyFieldProps, FormlyFieldSelectProps {
    checkAll?: boolean;
    childrenField?: string;
    kendoOptions?: any[];
    dataItems?: any[];
}

export interface FormlyMultiSelectTreeFieldConfig extends FormlyFieldConfig<MultiSelectTreeProps> {
    type: 'multiSelectTree' | Type<FormlyFieldMultiSelectTree>;
}

@Component({
    selector: 'formly-field-kendo-multiselecttree',
    template: `
    <kendo-multiselecttree
        kendoMultiSelectTreeExpandable
        [formControl]="formControl"
        [formlyAttributes]="field"
        [kendoMultiSelectTreeHierarchyBinding]="props.kendoOptions"
        [textField]="props.labelProp?.toString() || 'label'"
        [valueField]="props.valueProp?.toString() || 'value'"
        [valuePrimitive]="true"
        [readonly]="props.readonly === true"
        (valueChange)="valueChange(field, $event)"

        [dataItems]="props.dataItems"
        [childrenField]="props.childrenField"
        [checkAll]="props.checkAll"
        [filterable]="true"
        [tagMapper]="tagMapper"
    >
        <ng-template kendoMultiSelectTreeGroupTagTemplate let-dataItems>
            {{ dataItems.length }} item(s) selected
        </ng-template>
    </kendo-multiselecttree>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldMultiSelectTree extends FieldType<FieldTypeConfig<MultiSelectTreeProps>> {

    tagMapper(tags: any[]): any[] {
        return tags.length < 3 ? tags : [tags];
    }

    override defaultOptions = {
        hooks: {
            onInit: (field: FormlyFieldConfig<MultiSelectTreeProps>) => {
                const key = field.key as string;
                const options = field.props.options as any[];
                const values = field.model[key] as any[];
                const valueProp = field.props.valueProp?.toString() || 'value';

                field.props.dataItems = options.filter((item) => values.includes(item[valueProp]));
                field.props.kendoOptions = options;
            }
        }
    };

    valueChange(field: FormlyFieldConfig<MultiSelectTreeProps>, values: any) {
        //props.change && props.change(field, $event)

        setTimeout(() => {
            const check = !(field.props.dataItems.length == values.length);

            if (check) {
                const valueProp = field.props.valueProp?.toString() || 'value';
                const options = field.props.options as any[];

                field.props.dataItems = options.filter((item) => values.includes(item[valueProp]));
            }
        });

    }
}
