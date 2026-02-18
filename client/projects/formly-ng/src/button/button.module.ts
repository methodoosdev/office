import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { ButtonModule } from '@progress/kendo-angular-buttons';

import { FormlyFormFieldModule } from '../form-field/public_api';
import { FormlyFieldButton } from './button.type';

@NgModule({
    declarations: [FormlyFieldButton],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        ButtonModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'button',
                    component: FormlyFieldButton,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            hideLabel: true
                        },
                    }
                }
            ],
        }),
    ],
})
export class FormlyButtonModule { }
