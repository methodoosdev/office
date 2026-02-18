import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { FormlyFieldTextareaButton } from './textarea-button.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldTextareaButton],
    imports: [
        CommonModule,
        InputsModule,
        ButtonsModule,
        ReactiveFormsModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'textareaButton',
                    component: FormlyFieldTextareaButton,
                    wrappers: ['form-field'],
                },
            ],
        }),
    ],
})
export class FormlyTextareaButtonModule { }
