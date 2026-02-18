import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { FormlyFieldTextButton } from './text-button.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldTextButton],
    imports: [
        CommonModule,
        InputsModule,
        ButtonsModule,
        ReactiveFormsModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'textButton',
                    component: FormlyFieldTextButton,
                    wrappers: ['form-field'],
                },
            ],
        }),
    ],
})
export class FormlyTextButtonModule { }
