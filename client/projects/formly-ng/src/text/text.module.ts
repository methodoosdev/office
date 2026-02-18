import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FormlyFieldText } from './text.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldText],
    imports: [
        CommonModule,
        InputsModule,
        ReactiveFormsModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'text',
                    component: FormlyFieldText,
                    wrappers: ['form-field'],
                }
            ],
        }),
    ],
})
export class FormlyTextModule { }
