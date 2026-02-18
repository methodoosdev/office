import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormlySelectModule as FormlyCoreSelectModule } from '@ngx-formly/core/select';

import { FormlyFieldGridSelect } from './grid-select.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldGridSelect],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DropDownsModule,

        FormlyFormFieldModule,
        FormlyCoreSelectModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'gridSelect',
                    component: FormlyFieldGridSelect,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            options: [],
                            listHeight: 200,
                            labelProp: 'label',
                            valueProp: 'value'
                        }
                    }
                }
            ],
        }),
    ],
})
export class FormlyGridSelectModule { }
