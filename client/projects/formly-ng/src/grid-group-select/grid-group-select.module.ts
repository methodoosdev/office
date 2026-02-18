import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormlySelectModule as FormlyCoreSelectModule } from '@ngx-formly/core/select';

import { FormlyFieldGridGroupSelect } from './grid-group-select.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldGridGroupSelect],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DropDownsModule,

        FormlyFormFieldModule,
        FormlyCoreSelectModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'gridGroupSelect',
                    component: FormlyFieldGridGroupSelect,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            options: [],
                            listHeight: 200,
                            labelProp: 'label',
                            valueProp: 'value',
                            groupable: false,
                            groupProp: 'groupItem',
                            sourceOptions: []
                        }
                    }
                }
            ],
        }),
    ],
})
export class FormlyGridGroupSelectModule { }
