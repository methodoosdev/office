import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';

import { FormlyFieldFilterControl } from './filter-control.type';
import { FormlyFormFieldModule } from '../form-field/public_api';
import { FilterModule } from '@progress/kendo-angular-filter';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';

@NgModule({
    declarations: [FormlyFieldFilterControl],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FilterModule, DropDownsModule, DatePickerModule, ButtonsModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'filterControl',
                    component: FormlyFieldFilterControl,
                    wrappers: ['form-field']
                }
            ],
        }),
    ],
})
export class FormlyFilterControlModule { }
