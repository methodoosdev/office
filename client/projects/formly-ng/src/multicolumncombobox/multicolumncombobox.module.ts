import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { IntlModule } from "@progress/kendo-angular-intl";
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
//import { FormlySelectModule as FormlyCoreSelectModule } from '@ngx-formly/core/select';

import { FormlyFieldMultiColumnComboBox } from './multicolumncombobox.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldMultiColumnComboBox],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DropDownsModule,
        IntlModule,

        FormlyFormFieldModule,
        //FormlyCoreSelectModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'multiColumnComboBox',
                    component: FormlyFieldMultiColumnComboBox,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            disabledProp: 'active',
                        }
                    }
                }
            ],
        }),
    ],
})
export class FormlyMultiColumnComboBoxModule { }
