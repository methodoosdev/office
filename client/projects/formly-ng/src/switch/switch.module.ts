import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { LabelModule } from '@progress/kendo-angular-label';
import { InputsModule } from '@progress/kendo-angular-inputs';

import { FormlyFieldSwitch } from './switch.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldSwitch],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        InputsModule,
        LabelModule,
        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'switch',
                    component: FormlyFieldSwitch,
                    wrappers: ['form-field'],
                }
            ],
        }),
    ],
})
export class FormlySwitchModule { }
