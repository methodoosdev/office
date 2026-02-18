import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { LabelModule } from '@progress/kendo-angular-label';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormlySelectModule as FormlyCoreSelectModule } from '@ngx-formly/core/select';

import { FormlyFieldMultiSelectAll } from './multiSelectAll.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldMultiSelectAll],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DropDownsModule,
        LabelModule,
        InputsModule,

        FormlyFormFieldModule,
        FormlyCoreSelectModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'multiSelectAll',
                    component: FormlyFieldMultiSelectAll,
                    wrappers: ['form-field'],
                }
            ],
        }),
    ],
})
export class FormlyMultiSelectAllModule { }
