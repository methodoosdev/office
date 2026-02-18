import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { LabelModule } from '@progress/kendo-angular-label';
import { InputsModule } from '@progress/kendo-angular-inputs';

import { FormlyFieldCheckbox } from './checkbox.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldCheckbox],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        InputsModule,
        LabelModule,
        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'checkbox',
                    component: FormlyFieldCheckbox,
                    wrappers: ['form-field'],
                },
                {
                    name: 'boolean',
                    extends: 'checkbox',
                },
            ],
        }),
    ],
})
export class FormlyCheckboxModule { }
