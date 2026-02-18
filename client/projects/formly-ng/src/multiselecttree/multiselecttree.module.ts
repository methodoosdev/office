import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormlySelectModule as FormlyCoreSelectModule } from '@ngx-formly/core/select';

import { FormlyFieldMultiSelectTree } from './multiselecttree.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldMultiSelectTree],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DropDownsModule,

        FormlyFormFieldModule,
        FormlyCoreSelectModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'multiSelectTree',
                    component: FormlyFieldMultiSelectTree,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            checkAll: true,
                            childrenField: "items",
                            kendoOptions: [],
                            dataItems: []
                        }
                    }
                }
            ],
        }),
    ],
})
export class FormlyMultiSelectTreeModule { }
