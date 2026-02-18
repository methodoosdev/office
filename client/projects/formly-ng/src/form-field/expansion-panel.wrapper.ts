import { Component, Type } from '@angular/core';
import { FieldWrapper, FormlyFieldConfig } from '@ngx-formly/core';
import { BaseFormlyFieldProps } from './base-formly-field-props';

interface ExpansionPanelProps extends BaseFormlyFieldProps {
    expanded?: boolean;
}

//export interface FormlyExpansionPanelFieldConfig extends FormlyFieldConfig<ExpansionPanelProps> {
//    type: 'expansion-panel' | Type<FormlyWrapperExpansionPanel>;
//}

@Component({
    selector: 'formly-wrapper-expansion-panel',
    template: `
        <kendo-expansionpanel [title]="props.label" [expanded]="props.expanded === true">
            <ng-container #fieldComponent></ng-container>
        </kendo-expansionpanel>
    `
})
export class FormlyWrapperExpansionPanel extends FieldWrapper<FormlyFieldConfig<ExpansionPanelProps>> { }