import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FormlyFieldTextArea } from './textarea.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldTextArea],
    imports: [
        CommonModule,
        InputsModule,
        ReactiveFormsModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'textarea',
                    component: FormlyFieldTextArea,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            rows: 5,
                        },
                    }
                }
            ],
        }),
    ],
})
export class FormlyTextAreaModule { }
